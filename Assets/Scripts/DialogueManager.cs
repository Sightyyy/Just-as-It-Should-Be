using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class DialogueManager : MonoBehaviour
{
    [Header("Dialogue Parameters")]
    [SerializeField, Min(0f)] private float typingSpeed = 0.04f;
    [SerializeField] private InkFileManager inkFileManager;
    [SerializeField] private DialogueProgress dialogueProgress;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject continueIcon;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI displayNameText;
    [SerializeField] private Animator portraitAnimator;

    [Header("Choices UI")]
    [SerializeField] private GameObject[] choices;

    private readonly DialogueState dialogueState = new DialogueState();
    private readonly DialogueTagParser tagParser = new DialogueTagParser();
    private TextMeshProUGUI[] choiceTexts;
    private Button[] choiceButtons;
    private Story currentStory;
    private Coroutine displayLineCoroutine;
    private bool isTyping;
    private InkFileManager.ConversationBranch activeConversation;

    private static readonly string[] PersistentStatNames = { "calm", "fear", "doubt", "courage" };

    public bool DialogueIsPlaying { get; private set; }
    public static DialogueManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("More than one DialogueManager was found. The duplicate was disabled.", this);
            enabled = false;
            return;
        }

        Instance = this;
        dialogueProgress ??= GetComponent<DialogueProgress>();
        CacheChoiceControls();
    }

    public void ContinueDialogue()
    {
        if (!dialogueIsPlaying || !canContinueToNextLine || currentStory.currentChoices.Count > 0) return;

        ContinueStory();
    }

    private void Start()
    {
        SetDialogueVisible(false);
        HideChoices();
    }

    private void Update()
    {
        if (!DialogueIsPlaying || !Input.GetKeyDown(KeyCode.F))
        {
            return;
        }

        if (isTyping)
        {
            CompleteCurrentLine();
            return;
        }

        if (currentStory != null && currentStory.currentChoices.Count == 0)
        {
            ContinueStory();
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public bool StartConversation(string conversationKey)
    {
        if (DialogueIsPlaying)
        {
            return false;
        }

        if (inkFileManager == null)
        {
            Debug.LogError("DialogueManager needs an InkFileManager reference.", this);
            return false;
        }

        if (!inkFileManager.TryGetConversation(conversationKey, dialogueProgress, out InkFileManager.ConversationBranch conversation))
        {
            Debug.LogError($"Conversation '{conversationKey}' is missing, has no Ink file, or its conditions are not met.", this);
            return false;
        }

        BeginConversation(conversation);
        return true;
    }

    public bool StartBestConversation(IReadOnlyList<string> candidateKeys)
    {
        if (DialogueIsPlaying || inkFileManager == null)
        {
            return false;
        }

        if (!inkFileManager.TryGetBestConversation(candidateKeys, dialogueProgress, out InkFileManager.ConversationBranch conversation))
        {
            Debug.LogWarning("No eligible conversation is available for this NPC.", this);
            return false;
        }

        BeginConversation(conversation);
        return true;
    }

    private void BeginConversation(InkFileManager.ConversationBranch conversation)
    {
        activeConversation = conversation;
        inkFileManager.StartConversation(conversation.conversationKey);
        EnterDialogueMode(conversation.inkFile);
    }

    public void EnterDialogueMode(TextAsset inkJson)
    {
        if (inkJson == null)
        {
            Debug.LogError("Cannot start dialogue without an Ink JSON asset.", this);
            return;
        }

        StopActiveLine();
        currentStory = new Story(inkJson.text);
        BindProgressFunctions();
        ApplyPersistentStatsToInk();
        dialogueState.Reset();
        DialogueIsPlaying = true;
        SetDialogueVisible(true);
        HideChoices();

        if (displayNameText != null)
        {
            displayNameText.text = "???";
        }

        ContinueStory();
    }

    private void ContinueStory()
    {
        if (currentStory == null)
        {
            ExitDialogueMode();
            return;
        }

        if (!currentStory.canContinue)
        {
            if (currentStory.currentChoices.Count > 0)
            {
                DisplayChoices();
            }
            else
            {
                FinishConversation();
            }
            return;
        }

        string nextLine = currentStory.Continue();
        tagParser.Apply(currentStory.currentTags, dialogueState);
        ApplyPresentationTags();

        StopActiveLine();
        displayLineCoroutine = StartCoroutine(DisplayLine(nextLine));
    }

    private IEnumerator DisplayLine(string line)
    {
        if (dialogueText == null)
        {
            Debug.LogError("DialogueManager needs a dialogue text reference.", this);
            FinishConversation();
            yield break;
        }

        dialogueText.text = line;
        dialogueText.maxVisibleCharacters = 0;
        dialogueText.ForceMeshUpdate();
        int visibleCharacterCount = dialogueText.textInfo.characterCount;

        isTyping = true;
        SetContinueVisible(false);
        HideChoices();

        for (int visibleCharacters = 0; visibleCharacters < visibleCharacterCount; visibleCharacters++)
        {
            dialogueText.maxVisibleCharacters = visibleCharacters + 1;
            if (typingSpeed > 0f)
            {
                yield return new WaitForSeconds(typingSpeed);
            }
            else
            {
                yield return null;
            }
        }

        isTyping = false;
        displayLineCoroutine = null;
        SetContinueVisible(currentStory != null && currentStory.currentChoices.Count == 0);

        if (currentStory != null && currentStory.currentChoices.Count > 0)
        {
            DisplayChoices();
        }
    }

    private void CompleteCurrentLine()
    {
        if (dialogueText == null)
        {
            return;
        }

        StopActiveLine();
        dialogueText.maxVisibleCharacters = int.MaxValue;
        SetContinueVisible(currentStory != null && currentStory.currentChoices.Count == 0);

        if (currentStory != null && currentStory.currentChoices.Count > 0)
        {
            DisplayChoices();
        }
    }

    private void DisplayChoices()
    {
        if (currentStory == null)
        {
            return;
        }

        int supportedChoiceCount = Mathf.Min(currentStory.currentChoices.Count, choices.Length);
        if (currentStory.currentChoices.Count > choices.Length)
        {
            Debug.LogWarning($"Dialogue has {currentStory.currentChoices.Count} choices but this UI supports {choices.Length}. Extra choices are unavailable.", this);
        }

        for (int index = 0; index < choices.Length; index++)
        {
            bool shouldShow = index < supportedChoiceCount;
            if (choices[index] != null)
            {
                choices[index].SetActive(shouldShow);
            }

            if (shouldShow && choiceTexts[index] != null)
            {
                choiceTexts[index].text = currentStory.currentChoices[index].text;
            }
        }

        SetContinueVisible(false);
        SelectFirstVisibleChoice();
    }

    public void MakeChoice(int choiceIndex)
    {
        if (!DialogueIsPlaying || currentStory == null || isTyping || choiceIndex < 0 || choiceIndex >= currentStory.currentChoices.Count)
        {
            return;
        }

        HideChoices();
        currentStory.ChooseChoiceIndex(choiceIndex);
        ContinueStory();
    }

    private void ApplyPresentationTags()
    {
        if (displayNameText != null)
        {
            displayNameText.text = dialogueState.Speaker;
        }

        if (portraitAnimator == null || string.IsNullOrWhiteSpace(dialogueState.PortraitState))
        {
            return;
        }

        int stateHash = Animator.StringToHash(dialogueState.PortraitState);
        if (portraitAnimator.HasState(0, stateHash))
        {
            portraitAnimator.Play(stateHash);
        }
        else
        {
            Debug.LogWarning($"Portrait animator has no state named '{dialogueState.PortraitState}'.", portraitAnimator);
        }
    }

    private void CacheChoiceControls()
    {
        choiceTexts = new TextMeshProUGUI[choices?.Length ?? 0];
        choiceButtons = new Button[choices?.Length ?? 0];

        for (int index = 0; index < choiceTexts.Length; index++)
        {
            GameObject choice = choices[index];
            if (choice == null)
            {
                continue;
            }

            choiceTexts[index] = choice.GetComponentInChildren<TextMeshProUGUI>(true);
            choiceButtons[index] = choice.GetComponent<Button>();
            if (choiceButtons[index] == null)
            {
                Debug.LogWarning($"Dialogue choice {index + 1} has no Button component.", choice);
                continue;
            }

            int choiceIndex = index;
            choiceButtons[index].onClick.AddListener(() => MakeChoice(choiceIndex));
        }
    }

    private void HideChoices()
    {
        if (choices == null)
        {
            return;
        }

        foreach (GameObject choice in choices)
        {
            if (choice != null)
            {
                choice.SetActive(false);
            }
        }
    }

    private void SelectFirstVisibleChoice()
    {
        if (EventSystem.current == null || choices == null)
        {
            return;
        }

        EventSystem.current.SetSelectedGameObject(null);
        foreach (GameObject choice in choices)
        {
            if (choice != null && choice.activeInHierarchy)
            {
                EventSystem.current.SetSelectedGameObject(choice);
                return;
            }
        }
    }

    private void ExitDialogueMode()
    {
        StopActiveLine();
        DialogueIsPlaying = false;
        currentStory = null;
        activeConversation = null;
        dialogueState.Reset();
        HideChoices();
        SetContinueVisible(false);
        SetDialogueVisible(false);

        if (dialogueText != null)
        {
            dialogueText.text = string.Empty;
        }
    }

    private void StopActiveLine()
    {
        if (displayLineCoroutine != null)
        {
            StopCoroutine(displayLineCoroutine);
            displayLineCoroutine = null;
        }

        isTyping = false;
    }

    private void SetDialogueVisible(bool visible)
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(visible);
        }
    }

    private void SetContinueVisible(bool visible)
    {
        if (continueIcon != null)
        {
            continueIcon.SetActive(visible);
        }
    }

    private void BindProgressFunctions()
    {
        if (currentStory == null || dialogueProgress == null)
        {
            return;
        }

        currentStory.BindExternalFunction<string>("GetStat", statName => dialogueProgress.GetStat(statName));
        currentStory.BindExternalFunction<string, int>("AddStat", (statName, amount) => dialogueProgress.AddStat(statName, amount));
        currentStory.BindExternalFunction<string>("HasFlag", flag => dialogueProgress.HasFlag(flag));
        currentStory.BindExternalFunction<string>("SetFlag", flag => dialogueProgress.SetFlag(flag));
    }

    private void ApplyPersistentStatsToInk()
    {
        if (currentStory == null || dialogueProgress == null)
        {
            return;
        }

        foreach (string statName in PersistentStatNames)
        {
            if (currentStory.variablesState.GlobalVariableExistsWithName(statName))
            {
                currentStory.variablesState[statName] = dialogueProgress.GetStat(statName);
            }
        }
    }

    private void CaptureInkStats()
    {
        if (currentStory == null || dialogueProgress == null)
        {
            return;
        }

        foreach (string statName in PersistentStatNames)
        {
            if (currentStory.variablesState.GlobalVariableExistsWithName(statName))
            {
                object value = currentStory.variablesState[statName];
                if (value is int intValue)
                {
                    dialogueProgress.SetStat(statName, intValue);
                }
            }
        }
    }

    private void FinishConversation()
    {
        CaptureInkStats();

        string nextConversationKey = null;
        if (currentStory != null && currentStory.variablesState.GlobalVariableExistsWithName("nextBranch"))
        {
            nextConversationKey = currentStory.variablesState["nextBranch"] as string;
        }

        if (dialogueProgress != null && activeConversation != null && !string.IsNullOrWhiteSpace(activeConversation.completionFlag))
        {
            dialogueProgress.SetFlag(activeConversation.completionFlag);
        }

        if (!string.IsNullOrWhiteSpace(nextConversationKey) && inkFileManager != null &&
            inkFileManager.TryGetConversation(nextConversationKey, dialogueProgress, out InkFileManager.ConversationBranch nextConversation))
        {
            BeginConversation(nextConversation);
            return;
        }

        ExitDialogueMode();
    }
}
