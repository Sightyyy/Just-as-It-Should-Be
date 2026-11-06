using System;
using UnityEngine;

public enum SkillUpgradeType
{
    ConvincingDamage,
    FeelingsFortification,
    ClearMind
}

public sealed class SkillUpgradeManager : MonoBehaviour
{
    [SerializeField] private int convincingDamageLevel;
    [SerializeField] private int feelingsFortificationLevel;
    [SerializeField] private int clearMindLevel;

    private const int MaxConvincingDamageLevel = 4;
    private const int MaxFeelingsFortificationLevel = 3;
    private const int MaxClearMindLevel = 4;

    public static SkillUpgradeManager Instance { get; private set; }

    public int ConvincingDamageLevel => convincingDamageLevel;
    public int FeelingsFortificationLevel => feelingsFortificationLevel;
    public int ClearMindLevel => clearMindLevel;
    public int TypingDamagePerCorrectLetter => 1 + convincingDamageLevel;
    public float EmotionalDamageResistance => feelingsFortificationLevel <= 0 ? 0.15f : 0.15f + feelingsFortificationLevel * 0.05f;
    public float FuzzySpawnChanceReduction => clearMindLevel * 0.05f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("More than one SkillUpgradeManager was found. The duplicate was disabled.", this);
            enabled = false;
            return;
        }

        Instance = this;
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
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public bool Upgrade(SkillUpgradeType type)
    {
        switch (type)
        {
            case SkillUpgradeType.ConvincingDamage:
                return TryIncrement(ref convincingDamageLevel, MaxConvincingDamageLevel);
            case SkillUpgradeType.FeelingsFortification:
                return TryIncrement(ref feelingsFortificationLevel, MaxFeelingsFortificationLevel);
            case SkillUpgradeType.ClearMind:
                return TryIncrement(ref clearMindLevel, MaxClearMindLevel);
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    public void UpgradeConvincingDamage() => Upgrade(SkillUpgradeType.ConvincingDamage);
    public void UpgradeFeelingsFortification() => Upgrade(SkillUpgradeType.FeelingsFortification);
    public void UpgradeClearMind() => Upgrade(SkillUpgradeType.ClearMind);

    /// <summary>Current level of the given skill (0 = not upgraded at all).</summary>
    public int GetLevel(SkillUpgradeType type)
    {
        switch (type)
        {
            case SkillUpgradeType.ConvincingDamage:
                return convincingDamageLevel;
            case SkillUpgradeType.FeelingsFortification:
                return feelingsFortificationLevel;
            case SkillUpgradeType.ClearMind:
                return clearMindLevel;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    /// <summary>Max obtainable level for the given skill.</summary>
    public int GetMaxLevel(SkillUpgradeType type)
    {
        switch (type)
        {
            case SkillUpgradeType.ConvincingDamage:
                return MaxConvincingDamageLevel;
            case SkillUpgradeType.FeelingsFortification:
                return MaxFeelingsFortificationLevel;
            case SkillUpgradeType.ClearMind:
                return MaxClearMindLevel;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    public void WriteTo(SaveData data)
    {
        if (data == null) return;

        data.convincingDamageLevel = convincingDamageLevel;
        data.feelingsFortificationLevel = feelingsFortificationLevel;
        data.clearMindLevel = clearMindLevel;
    }

    public void ReadFrom(SaveData data)
    {
        if (data == null) return;

        convincingDamageLevel = Mathf.Clamp(data.convincingDamageLevel, 0, MaxConvincingDamageLevel);
        feelingsFortificationLevel = Mathf.Clamp(data.feelingsFortificationLevel, 0, MaxFeelingsFortificationLevel);
        clearMindLevel = Mathf.Clamp(data.clearMindLevel, 0, MaxClearMindLevel);
    }

    private static bool TryIncrement(ref int level, int maxLevel)
    {
        if (level >= maxLevel) return false;

        level++;
        return true;
    }
}