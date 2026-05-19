using UnityEngine;
using UnityEngine.UI;
using Ink.Runtime;
using TMPro;

public class InkUIManager : MonoBehaviour
{
    public TextAsset inkJSON;

    private Story story;

    [Header("UI")]
    public TMP_Text dialogueText;
    public Button[] choiceButtons; // isi 4 button di inspector

    private bool isDialogueActive = false;
    private bool waitingForInput = false;

    void Update()
    {
        if (!isDialogueActive && Input.GetKeyDown(KeyCode.F))
        {
            StartDialogue();
        }
        else if (isDialogueActive && waitingForInput && Input.GetKeyDown(KeyCode.F))
        {
            ContinueStory();
        }
    }

    void StartDialogue()
    {
        story = new Story(inkJSON.text);
        isDialogueActive = true;

        ContinueStory();
    }

    void ContinueStory()
    {
        if (story.canContinue)
        {
            string text = story.Continue().Trim();

            if (string.IsNullOrEmpty(text))
            {
                ContinueStory();
                return;
            }

            dialogueText.text = text;

            HandleTags();

            waitingForInput = true;

            if (story.currentChoices.Count > 0)
            {
                waitingForInput = false;
                ShowChoices();
            }
            else
            {
                HideChoices();
            }
        }
        else
        {
            EndDialogue();
        }
    }

    void ShowChoices()
    {
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < story.currentChoices.Count)
            {
                choiceButtons[i].gameObject.SetActive(true);

                TMP_Text btnText = choiceButtons[i].GetComponentInChildren<TMP_Text>();
                btnText.text = story.currentChoices[i].text;

                int choiceIndex = i; // penting (closure fix)
                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() => ChooseChoice(choiceIndex));
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }
    }

    void HideChoices()
    {
        foreach (var btn in choiceButtons)
        {
            btn.gameObject.SetActive(false);
        }
    }

    public void ChooseChoice(int index)
    {
        story.ChooseChoiceIndex(index);
        ContinueStory();
    }

    void HandleTags()
    {
        foreach (string tag in story.currentTags)
        {
            Debug.Log("TAG: " + tag);
        }
    }

    void EndDialogue()
    {
        dialogueText.text = "";
        HideChoices();
        isDialogueActive = false;
    }
}