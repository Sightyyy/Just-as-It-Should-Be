using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class PatchingUpMinigameController : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private PlayerStates playerStates;
    [SerializeField] private RealmManager realmManager;
    [SerializeField] private FuzzyCleaningChallenge fuzzyCleaning;
    [SerializeField] private SkillUpgradeManager skillUpgradeManager;

    [Header("Wounds")]
    [SerializeField] private GameObject woundPrefab;
    [SerializeField] private GameObject fuzzyVisualPrefab;
    [SerializeField] private List<Transform> woundSpawnPoints = new List<Transform>();
    [SerializeField] private List<WoundConditionRule> conditionRules = new List<WoundConditionRule>
    {
        new WoundConditionRule(PlayerEmotionalState.Afraid, 15f, 0.6f, 10, PlayerEmotionalState.Sad),
        new WoundConditionRule(PlayerEmotionalState.Sad, 10f, 0.8f, 40, PlayerEmotionalState.Depressed),
        new WoundConditionRule(PlayerEmotionalState.Depressed, 5f, 1f, 65, PlayerEmotionalState.DeathEnding)
    };
    [SerializeField] private int woundReductionUpgrade = 0;

    [Header("Patching UI")]
    [SerializeField] private GameObject patchingPanel;
    [SerializeField] private Slider patchProgressSlider;
    [SerializeField] private TextMeshProUGUI woundCountText;
    [SerializeField] private TextMeshProUGUI patchedCountText;
    [SerializeField] private KeyCode patchKey = KeyCode.Z;
    [SerializeField] private float holdSecondsToPatch = 5f;

    private static readonly PlayerEmotionalState[] WoundStatesInPriorityOrder =
    {
        PlayerEmotionalState.Depressed,
        PlayerEmotionalState.Sad,
        PlayerEmotionalState.Afraid
    };

    private readonly List<PatchingWound> wounds = new List<PatchingWound>();

    private float woundTimer;
    private float patchHoldTimer;
    private int spawnPointIndex;
    private int totalPatchedWounds;
    private bool isPatchingActive;

    public int ActiveWoundCount => wounds.Count;
    public int TotalPatchedWounds => totalPatchedWounds;
    public bool HasPatchableWound => GetNextWound() != null && GetNextWound().IsPatchable;
    public bool IsPatchingActive => isPatchingActive;
    public static bool HasAnyActiveWounds { get; private set; }

    void Reset()
    {
        conditionRules = new List<WoundConditionRule>
        {
            new WoundConditionRule(PlayerEmotionalState.Afraid, 15f, 0.6f, 10, PlayerEmotionalState.Sad),
            new WoundConditionRule(PlayerEmotionalState.Sad, 10f, 0.8f, 40, PlayerEmotionalState.Depressed),
            new WoundConditionRule(PlayerEmotionalState.Depressed, 5f, 1f, 65, PlayerEmotionalState.DeathEnding)
        };
    }

    void Awake()
    {
        playerStates ??= PlayerStates.Instance;
        fuzzyCleaning ??= GetComponent<FuzzyCleaningChallenge>();
        skillUpgradeManager ??= SkillUpgradeManager.Instance;
    }

    void OnEnable()
    {
        SaveManager.BeforeSave += WriteTo;
        SaveManager.AfterLoad += ReadFrom;
    }

    void OnDisable()
    {
        SaveManager.BeforeSave -= WriteTo;
        SaveManager.AfterLoad -= ReadFrom;

        if (HasAnyActiveWounds && wounds.Count > 0)
        {
            HasAnyActiveWounds = false;
        }
    }

    void Start()
    {
        SetPatchingPanel(false);
        RefreshUI();
    }

    void Update()
    {
        WoundConditionRule currentRule = GetCurrentRule();
        if (currentRule == null)
        {
            ResetPatchingProgress();
            return;
        }

        GenerateWounds(currentRule);
        EvaluateEscalation(currentRule);

        if (isPatchingActive)
        {
            HandleCurrentWound();
        }

        RefreshUI();
    }

    public bool CanBeginPatching()
    {
        return GetCurrentRule() != null && wounds.Count > 0;
    }

    public void BeginPatching()
    {
        if (!CanBeginPatching()) return;

        isPatchingActive = true;
        SetPatchingPanel(true);
    }

    public void StopPatching()
    {
        isPatchingActive = false;
        ResetPatchingProgress();
        fuzzyCleaning?.Cancel();
        SetPatchingPanel(false);
    }

    public void SetWoundReductionUpgrade(int reduction)
    {
        woundReductionUpgrade = Mathf.Max(0, reduction);
    }

    public void SpawnDebugWound(bool hasFuzzy)
    {
        SpawnWound(hasFuzzy);
        RefreshUI();
    }

    public void WriteTo(SaveData data)
    {
        if (data == null) return;

        data.activeWoundCount = wounds.Count;
        data.totalPatchedWounds = totalPatchedWounds;
    }

    public void ReadFrom(SaveData data)
    {
        if (data == null) return;

        ClearWounds();
        totalPatchedWounds = Mathf.Max(0, data.totalPatchedWounds);

        int count = Mathf.Max(0, data.activeWoundCount);
        for (int i = 0; i < count; i++)
        {
            SpawnWound(false);
        }

        RefreshUI();
    }

    private void GenerateWounds(WoundConditionRule rule)
    {
        if (realmManager != null && realmManager.CurrentRealm != RealmManager.Realm.Inner) return;

        woundTimer += Time.deltaTime * (1f - GetEmotionalDamageResistance());
        float interval = GetAdjustedWoundInterval(rule);

        while (woundTimer >= interval)
        {
            woundTimer -= interval;
            bool hasFuzzy = Random.value <= GetAdjustedFuzzySpawnChance(rule);
            SpawnWound(hasFuzzy);
        }
    }

    private void HandleCurrentWound()
    {
        PatchingWound wound = GetNextWound();
        if (wound == null)
        {
            ResetPatchingProgress();
            return;
        }

        SetPatchingPanel(true);

        if (wound.HasFuzzy)
        {
            if (fuzzyCleaning != null && !fuzzyCleaning.IsActive)
            {
                fuzzyCleaning.Begin(wound);
            }

            ResetPatchingProgress();
            return;
        }

        if (Input.GetKey(patchKey))
        {
            patchHoldTimer += Time.deltaTime;

            if (patchHoldTimer >= holdSecondsToPatch)
            {
                PatchWound(wound);
            }
        }
        else
        {
            patchHoldTimer = 0f;
        }
    }

    private void PatchWound(PatchingWound wound)
    {
        if (wound == null || wound.HasFuzzy) return;

        wounds.Remove(wound);

        if (wound.SpawnedObject != null)
        {
            Destroy(wound.SpawnedObject);
        }

        totalPatchedWounds++;
        patchHoldTimer = 0f;
    }

    private void EvaluateEscalation(WoundConditionRule rule)
    {
        if (wounds.Count <= rule.EscalationThreshold) return;

        playerStates?.SetActive(rule.EscalatesTo, true);
    }

    private void SpawnWound(bool hasFuzzy)
    {
        GameObject woundObject = null;
        Transform spawnPoint = GetNextSpawnPoint();

        if (woundPrefab != null)
        {
            Vector3 position = spawnPoint != null ? spawnPoint.position : transform.position;
            Quaternion rotation = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;
            woundObject = Instantiate(woundPrefab, position, rotation, spawnPoint);
        }

        GameObject fuzzyObject = null;
        if (hasFuzzy && fuzzyVisualPrefab != null && woundObject != null)
        {
            fuzzyObject = Instantiate(fuzzyVisualPrefab, woundObject.transform);
        }

        wounds.Add(new PatchingWound(woundObject, fuzzyObject));
    }

    private Transform GetNextSpawnPoint()
    {
        if (woundSpawnPoints.Count == 0) return null;

        Transform spawnPoint = woundSpawnPoints[spawnPointIndex % woundSpawnPoints.Count];
        spawnPointIndex++;
        return spawnPoint;
    }

    private PatchingWound GetNextWound()
    {
        return wounds.Count > 0 ? wounds[0] : null;
    }

    private WoundConditionRule GetCurrentRule()
    {
        if (playerStates == null) return null;

        foreach (PlayerEmotionalState state in WoundStatesInPriorityOrder)
        {
            if (!playerStates.IsActive(state)) continue;

            foreach (WoundConditionRule rule in conditionRules)
            {
                if (rule != null && rule.State == state)
                {
                    return rule;
                }
            }
        }

        return null;
    }

    private float GetAdjustedWoundInterval(WoundConditionRule rule)
    {
        return rule.SecondsPerWound + woundReductionUpgrade;
    }

    private float GetAdjustedFuzzySpawnChance(WoundConditionRule rule)
    {
        if (skillUpgradeManager == null)
        {
            skillUpgradeManager = SkillUpgradeManager.Instance;
        }

        float reduction = skillUpgradeManager != null ? skillUpgradeManager.FuzzySpawnChanceReduction : 0f;
        return Mathf.Clamp01(rule.FuzzySpawnChance - reduction);
    }

    private float GetEmotionalDamageResistance()
    {
        if (skillUpgradeManager == null)
        {
            skillUpgradeManager = SkillUpgradeManager.Instance;
        }

        return skillUpgradeManager != null ? Mathf.Clamp01(skillUpgradeManager.EmotionalDamageResistance) : 0f;
    }

    private void ResetPatchingProgress()
    {
        patchHoldTimer = 0f;
        if (wounds.Count == 0)
        {
            isPatchingActive = false;
            SetPatchingPanel(false);
        }
    }

    private void ClearWounds()
    {
        foreach (PatchingWound wound in wounds)
        {
            if (wound.SpawnedObject != null)
            {
                Destroy(wound.SpawnedObject);
            }
        }

        wounds.Clear();
        patchHoldTimer = 0f;
        woundTimer = 0f;
    }

    private void RefreshUI()
    {
        if (patchProgressSlider != null)
        {
            patchProgressSlider.value = patchHoldTimer / Mathf.Max(0.1f, holdSecondsToPatch);
        }

        if (woundCountText != null)
        {
            woundCountText.text = wounds.Count.ToString();
        }

        HasAnyActiveWounds = wounds.Count > 0;

        if (patchedCountText != null)
        {
            patchedCountText.text = totalPatchedWounds.ToString();
        }
    }

    private void SetPatchingPanel(bool active)
    {
        if (patchingPanel != null)
        {
            patchingPanel.SetActive(active);
        }
    }
}
