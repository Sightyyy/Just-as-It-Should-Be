using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class TypingMiniGameController : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private PlayerStates playerStates;
    [SerializeField] private RealmManager realmManager;
    [SerializeField] private OxfordTypingDictionary dictionary;
    [SerializeField] private List<PlayerMovement> playerMovements = new List<PlayerMovement>();

    [Header("Monsters")]
    [SerializeField] private float monsterRegenPercentPerSecond = 0.01f;
    [SerializeField] private List<TypingMonsterDefinition> monsterDefinitions = new List<TypingMonsterDefinition>();
    [Tooltip("Used only if a monster definition has no EnemyArea assigned.")]
    [SerializeField] private Transform fallbackSpawnPoint;
    [Tooltip("Local height offset (above the spawned monster) reserved for a future world-space health bar.")]
    [SerializeField] private float healthBarAnchorHeight = 1.5f;

    [Header("UI")]
    [SerializeField] private GameObject typingPanel;
    [SerializeField] private TextMeshProUGUI targetWordText;
    [SerializeField] private TextMeshProUGUI accuracyText;
    [SerializeField] private Image healthFillImage;

    [Header("Live Feedback Colors")]
    [SerializeField] private Color correctLetterColor = new Color(0.30f, 0.69f, 0.31f);
    [SerializeField] private Color wrongLetterColor = new Color(0.96f, 0.26f, 0.21f);
    [SerializeField] private Color untypedLetterColor = Color.black;

    private static readonly PlayerEmotionalState[] MonsterTriggerStatesInPriorityOrder =
    {
        PlayerEmotionalState.Depressed,
        PlayerEmotionalState.Sad,
        PlayerEmotionalState.Afraid
    };

    private readonly TypingAccuracyTracker accuracyTracker = new TypingAccuracyTracker();
    private readonly Dictionary<PlayerEmotionalState, MonsterEncounter> activeEncounters = new Dictionary<PlayerEmotionalState, MonsterEncounter>();

    private readonly HashSet<PlayerEmotionalState> defeatedPendingDeactivation = new HashSet<PlayerEmotionalState>();

    private MonsterEncounter currentEncounter;
    private string currentWord = string.Empty;
    private string typedSoFar = string.Empty;
    private bool isTypingActive;

    public bool HasLivingMonster => currentEncounter != null && !currentEncounter.Monster.IsDead;
    public bool IsTypingActive => isTypingActive;
    public PlayerEmotionalState? ActiveMonsterState => currentEncounter?.State;
    
    public static bool IsAnyMinigameActive { get; private set; }

    void Awake()
    {
        playerStates ??= PlayerStates.Instance;
        dictionary ??= GetComponent<OxfordTypingDictionary>();
    }

    void OnDestroy()
    {
        if (isTypingActive)
        {
            IsAnyMinigameActive = false;
            SetAllPlayerMovementControl(true);
        }
    }

    private void SetTypingActive(bool value)
    {
        isTypingActive = value;
        IsAnyMinigameActive = value;
        SetAllPlayerMovementControl(!value);
    }

    private void SetAllPlayerMovementControl(bool canControl)
    {
        foreach (PlayerMovement movement in playerMovements)
        {
            movement?.SetControl(canControl);
        }
    }

    void Start()
    {
        SetTypingPanel(false);
        RefreshUI();
    }

    void Update()
    {
        SyncActiveEncounters();

        if (isTypingActive)
        {
            HandleTypingInput();
        }
        else if (ShouldRegenerateMonsters())
        {
            RegenerateAllMonsters(Time.deltaTime);
            RefreshUI();
        }
    }

    public bool CanInteract()
    {
        return GetNextLivingEncounter() != null;
    }

    public void Interact()
    {
        MonsterEncounter next = GetNextLivingEncounter();
        if (next == null) return;

        currentEncounter = next;
        SetTypingActive(true);
        SetTypingPanel(true);
        accuracyTracker.Reset();
        RequestNextWord();
        RefreshUI();
    }

    public void StopTyping()
    {
        SetTypingActive(false);
        SetTypingPanel(false);
        typedSoFar = string.Empty;
    }

    private void SyncActiveEncounters()
    {
        if (playerStates == null) return;

        IReadOnlyCollection<PlayerEmotionalState> active = playerStates.GetActiveStates();

        foreach (PlayerEmotionalState state in MonsterTriggerStatesInPriorityOrder)
        {
            bool isActive = active.Contains(state);

            if (!isActive)
            {
                defeatedPendingDeactivation.Remove(state);
                DespawnEncounter(state);
                continue;
            }

            if (activeEncounters.ContainsKey(state)) continue;
            if (defeatedPendingDeactivation.Contains(state)) continue;

            TrySpawnEncounter(state);
        }
    }

    private void TrySpawnEncounter(PlayerEmotionalState state)
    {
        TypingMonsterDefinition definition = GetDefinition(state);
        if (definition == null)
        {
            Debug.LogWarning($"No typing monster definition was configured for {state}.", this);
            return;
        }

        bool isTraumatized = playerStates != null && playerStates.IsActive(PlayerEmotionalState.Traumatized);
        TypingMonster monster = new TypingMonster(state, definition.GetMaxHealth(isTraumatized), monsterRegenPercentPerSecond);

        GameObject spawnedObject = null;
        Transform healthBarAnchor = null;

        if (definition.Prefab != null)
        {
            Transform parent = definition.EnemyArea != null ? definition.EnemyArea : fallbackSpawnPoint;
            Vector3 position = parent != null ? parent.position : transform.position;
            Quaternion rotation = parent != null ? parent.rotation : Quaternion.identity;

            spawnedObject = Instantiate(definition.Prefab, position, rotation, parent);

            GameObject anchorObject = new GameObject("HealthBarAnchor");
            anchorObject.transform.SetParent(spawnedObject.transform, false);
            anchorObject.transform.localPosition = new Vector3(0f, healthBarAnchorHeight, 0f);
            healthBarAnchor = anchorObject.transform;
        }

        activeEncounters[state] = new MonsterEncounter(state, monster, spawnedObject, healthBarAnchor);
    }

    private void DespawnEncounter(PlayerEmotionalState state)
    {
        if (!activeEncounters.TryGetValue(state, out MonsterEncounter encounter)) return;

        activeEncounters.Remove(state);

        if (encounter.SpawnedObject != null)
        {
            Destroy(encounter.SpawnedObject);
        }

        if (currentEncounter == encounter)
        {
            currentEncounter = null;
            if (isTypingActive)
            {
                AdvanceOrEndSession();
            }
        }
    }

    private MonsterEncounter GetNextLivingEncounter()
    {
        if (currentEncounter != null && !currentEncounter.Monster.IsDead)
        {
            return currentEncounter;
        }

        foreach (PlayerEmotionalState state in MonsterTriggerStatesInPriorityOrder)
        {
            if (activeEncounters.TryGetValue(state, out MonsterEncounter encounter) && !encounter.Monster.IsDead)
            {
                return encounter;
            }
        }

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

    private void HandleTypingInput()
    {
        if (currentEncounter == null || currentEncounter.Monster.IsDead || string.IsNullOrEmpty(currentWord))
        {
            return;
        }

        string frameInput = Input.inputString;
        if (!string.IsNullOrEmpty(frameInput))
        {
            bool changed = false;

            foreach (char typedChar in frameInput)
            {
                if (typedSoFar.Length >= currentWord.Length) break;
                if (!char.IsLetter(typedChar)) continue;

                typedSoFar += typedChar;
                changed = true;
            }

            if (changed)
            {
                UpdateLiveWordColoring();
            }
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            SubmitTypedWord();
        }
    }

    private void SubmitTypedWord()
    {
        if (!isTypingActive || currentEncounter == null || currentEncounter.Monster.IsDead || string.IsNullOrWhiteSpace(currentWord))
        {
            return;
        }

        int damage = accuracyTracker.RecordAttempt(currentWord, typedSoFar);
        currentEncounter.Monster.Damage(damage);

        if (currentEncounter.Monster.IsDead)
        {
            HandleMonsterDefeated();
            return;
        }

        RequestNextWord();
        RefreshUI();
    }

    private void HandleMonsterDefeated()
    {
        ApplyPostEncounterRewards(accuracyTracker.AccuracyPercent);

        PlayerEmotionalState defeatedState = currentEncounter.State;
        defeatedPendingDeactivation.Add(defeatedState);
        DespawnDefeatedEncounter(defeatedState);

        AdvanceOrEndSession();
    }

    private void DespawnDefeatedEncounter(PlayerEmotionalState state)
    {
        if (activeEncounters.TryGetValue(state, out MonsterEncounter encounter))
        {
            activeEncounters.Remove(state);
            if (encounter.SpawnedObject != null)
            {
                Destroy(encounter.SpawnedObject);
            }
        }

        currentEncounter = null;
    }

    private void AdvanceOrEndSession()
    {
        MonsterEncounter next = GetNextLivingEncounter();

        if (next == null)
        {
            SetTypingActive(false);
            SetTypingPanel(false);
            currentWord = string.Empty;
            typedSoFar = string.Empty;
            RefreshUI();
            return;
        }

        currentEncounter = next;
        accuracyTracker.Reset();
        RequestNextWord();
        RefreshUI();
    }

    private void ApplyPostEncounterRewards(float accuracy)
    {
        if (accuracy >= 95f)
        {
            playerStates?.SetActive(PlayerEmotionalState.Confident, true);
            playerStates?.SetActive(PlayerEmotionalState.Traumatized, false);
        }
        else if (accuracy >= 75f)
        {
            playerStates?.SetActive(PlayerEmotionalState.Neutral, true);
        }
    }

    private bool ShouldRegenerateMonsters()
    {
        return realmManager != null && realmManager.CurrentRealm == RealmManager.Realm.Real;
    }

    private void RegenerateAllMonsters(float deltaTime)
    {
        foreach (MonsterEncounter encounter in activeEncounters.Values)
        {
            encounter.Monster.Regenerate(deltaTime);
        }
    }

    private void RequestNextWord()
    {
        currentWord = dictionary != null ? dictionary.GetRandomWord() : "texture";
        typedSoFar = string.Empty;
        UpdateLiveWordColoring();
    }

    private void UpdateLiveWordColoring()
    {
        if (targetWordText == null || string.IsNullOrEmpty(currentWord)) return;

        StringBuilder builder = new StringBuilder(currentWord.Length * 8);

        for (int i = 0; i < currentWord.Length; i++)
        {
            char targetChar = currentWord[i];
            Color color;

            if (i < typedSoFar.Length)
            {
                bool isCorrect = char.ToLowerInvariant(targetChar) == char.ToLowerInvariant(typedSoFar[i]);
                color = isCorrect ? correctLetterColor : wrongLetterColor;
            }
            else
            {
                color = untypedLetterColor;
            }

            builder.Append("<color=#").Append(ColorUtility.ToHtmlStringRGB(color)).Append('>')
                   .Append(targetChar)
                   .Append("</color>");
        }

        targetWordText.text = builder.ToString();
    }

    private void RefreshUI()
    {
        if (healthFillImage != null)
        {
            healthFillImage.fillAmount = currentEncounter != null ? currentEncounter.Monster.HealthNormalized : 0f;
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

    private void SetTypingPanel(bool active)
    {
        if (typingPanel != null)
        {
            typingPanel.SetActive(active);
        }
    }
}