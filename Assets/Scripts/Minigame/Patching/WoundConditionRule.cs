using System;
using UnityEngine;

[Serializable]
public sealed class WoundConditionRule
{
    [SerializeField] private PlayerEmotionalState state;
    [SerializeField] private float secondsPerWound = 15f;
    [SerializeField, Range(0f, 1f)] private float fuzzySpawnChance = 0.6f;
    [SerializeField] private int escalationThreshold = 10;
    [SerializeField] private PlayerEmotionalState escalatesTo = PlayerEmotionalState.Sad;

    public PlayerEmotionalState State => state;
    public float SecondsPerWound => Mathf.Max(0.1f, secondsPerWound);
    public float FuzzySpawnChance => Mathf.Clamp01(fuzzySpawnChance);
    public int EscalationThreshold => Mathf.Max(1, escalationThreshold);
    public PlayerEmotionalState EscalatesTo => escalatesTo;

    public WoundConditionRule()
    {
    }

    public WoundConditionRule(PlayerEmotionalState state, float secondsPerWound, float fuzzySpawnChance, int escalationThreshold, PlayerEmotionalState escalatesTo)
    {
        this.state = state;
        this.secondsPerWound = secondsPerWound;
        this.fuzzySpawnChance = fuzzySpawnChance;
        this.escalationThreshold = escalationThreshold;
        this.escalatesTo = escalatesTo;
    }
}
