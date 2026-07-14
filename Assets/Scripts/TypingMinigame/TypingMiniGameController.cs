using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class TypingMiniGameController : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private PlayerStates playerStates;
    [SerializeField] private RealmManager realmManager;
    [SerializeField] private OxfordTypingDictionary dictionary;

    [Header("Monster")]
    [SerializeField] private Transform monsterSpawnPoint;
    [SerializeField] private float monsterRegenPercentPerSecond = 0.01f;
    [SerializeField] private List<TypingMonsterDefinition> monsterDefinitions = new List<TypingMonsterDefinition>();

    [Header("UI")]
    [SerializeField] private GameObject typingPanel;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TextMeshProUGUI targetWordText;
    [SerializeField] private TextMeshProUGUI accuracyText;
    [SerializeField] private Slider healthBar;

    private readonly TypingAccuracyTracker accuracyTracker = new TypingAccuracyTracker();

    private TypingMonster activeMonster;
    private GameObject activeMonsterObject;
    private PlayerEmotionalState? activeMonsterState;
    private string currentWord;
    private bool isTypingActive;

    public bool HasLivingMonster => activeMonster != null && !activeMonster.IsDead;
    public bool IsTypingActive => isTypingActive;
    public PlayerEmotionalState? ActiveMonsterState => activeMonsterState;

    void Awake()
    {
        playerStates ??= PlayerStates.Instance;
        dictionary ??= GetComponent<OxfordTypingDictionary>();

        if (inputField != null)
        {
            inputField.onSubmit.AddListener(SubmitInput);
        }
    }

    void OnDestroy()
    {
        if (inputField != null)
        {
            inputField.onSubmit.RemoveListener(SubmitInput);
        }
    }

    void Start()
    {
        SetTypingPanel(false);
        RefreshUI();
    }

    void Update()
    {
        PlayerEmotionalState? currentState = ResolveCurrentMonsterState();

        if (HasLivingMonster && currentState != activeMonsterState)
        {
            ResetEncounterForState(currentState);
        }

        if (HasLivingMonster && ShouldRegenerateMonster())
        {
            activeMonster.Regenerate(Time.deltaTime);
            RefreshUI();
        }
    }

    public bool CanInteract()
    {
        return ResolveCurrentMonsterState().HasValue;
    }

    public void Interact()
    {
        PlayerEmotionalState? currentState = ResolveCurrentMonsterState();
        if (!currentState.HasValue) return;

        if (!HasLivingMonster || activeMonsterState != currentState.Value)
        {
            ResetEncounterForState(currentState.Value);
        }

        isTypingActive = true;
        SetTypingPanel(true);
        RequestNextWord();
        FocusInput();
    }

    public void StopTyping()
    {
        isTypingActive = false;
        SetTypingPanel(false);
    }

    private void SubmitInput(string playerInput)
    {
        if (!isTypingActive || !HasLivingMonster || string.IsNullOrWhiteSpace(currentWord)) return;

        int damage = accuracyTracker.RecordAttempt(currentWord, playerInput.Trim());
        activeMonster.Damage(damage);

        if (inputField != null)
        {
            inputField.text = string.Empty;
        }

        if (activeMonster.IsDead)
        {
            CompleteEncounter();
            return;
        }

        RequestNextWord();
        RefreshUI();
        FocusInput();
    }

    private void ResetEncounterForState(PlayerEmotionalState? state)
    {
        DestroyActiveMonsterObject();
        accuracyTracker.Reset();
        activeMonster = null;
        activeMonsterState = null;
        currentWord = string.Empty;

        if (!state.HasValue)
        {
            StopTyping();
            RefreshUI();
            return;
        }

        TypingMonsterDefinition definition = GetDefinition(state.Value);
        if (definition == null)
        {
            Debug.LogWarning($"No typing monster definition was configured for {state.Value}.", this);
            StopTyping();
            RefreshUI();
            return;
        }

        bool isTraumatized = playerStates != null && playerStates.IsActive(PlayerEmotionalState.Traumatized);
        activeMonster = new TypingMonster(state.Value, definition.GetMaxHealth(isTraumatized), monsterRegenPercentPerSecond);
        activeMonsterState = state.Value;
        SpawnMonster(definition);
        RefreshUI();
    }

    private void CompleteEncounter()
    {
        float accuracy = accuracyTracker.AccuracyPercent;

        if (accuracy >= 95f)
        {
            playerStates?.SetActive(PlayerEmotionalState.Confident, true);
            playerStates?.SetActive(PlayerEmotionalState.Traumatized, false);
        }
        else if (accuracy >= 75f)
        {
            playerStates?.SetActive(PlayerEmotionalState.Neutral, true);
        }

        DestroyActiveMonsterObject();
        activeMonster = null;
        activeMonsterState = null;
        currentWord = string.Empty;
        StopTyping();
        RefreshUI();
    }

    private bool ShouldRegenerateMonster()
    {
        return !isTypingActive && realmManager != null && realmManager.CurrentRealm == RealmManager.Realm.Real;
    }

    private PlayerEmotionalState? ResolveCurrentMonsterState()
    {
        if (playerStates == null) return null;

        if (playerStates.IsActive(PlayerEmotionalState.Depressed)) return PlayerEmotionalState.Depressed;
        if (playerStates.IsActive(PlayerEmotionalState.Sad)) return PlayerEmotionalState.Sad;
        if (playerStates.IsActive(PlayerEmotionalState.Afraid)) return PlayerEmotionalState.Afraid;

        return null;
    }

    private TypingMonsterDefinition GetDefinition(PlayerEmotionalState state)
    {
        foreach (TypingMonsterDefinition definition in monsterDefinitions)
        {
            if (definition != null && definition.State == state)
            {
                return definition;
            }
        }

        return null;
    }

    private void SpawnMonster(TypingMonsterDefinition definition)
    {
        if (definition.Prefab == null) return;

        Vector3 position = monsterSpawnPoint != null ? monsterSpawnPoint.position : transform.position;
        Quaternion rotation = monsterSpawnPoint != null ? monsterSpawnPoint.rotation : Quaternion.identity;
        activeMonsterObject = Instantiate(definition.Prefab, position, rotation);
    }

    private void DestroyActiveMonsterObject()
    {
        if (activeMonsterObject != null)
        {
            Destroy(activeMonsterObject);
            activeMonsterObject = null;
        }
    }

    private void RequestNextWord()
    {
        currentWord = dictionary != null ? dictionary.GetRandomWord() : "texture";

        if (targetWordText != null)
        {
            targetWordText.text = currentWord;
        }
    }

    private void RefreshUI()
    {
        if (healthBar != null)
        {
            healthBar.value = activeMonster != null ? activeMonster.HealthNormalized : 0f;
        }

        if (accuracyText != null)
        {
            accuracyText.text = $"{accuracyTracker.AccuracyPercent:0}%";
        }

        if (targetWordText != null && string.IsNullOrEmpty(currentWord))
        {
            targetWordText.text = string.Empty;
        }
    }

    private void FocusInput()
    {
        if (inputField == null) return;

        inputField.ActivateInputField();
        inputField.Select();
    }

    private void SetTypingPanel(bool active)
    {
        if (typingPanel != null)
        {
            typingPanel.SetActive(active);
        }
    }
}
