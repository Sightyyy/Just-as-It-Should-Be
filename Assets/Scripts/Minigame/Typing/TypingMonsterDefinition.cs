using System;
using UnityEngine;

[Serializable]
public sealed class TypingMonsterDefinition
{
    [SerializeField] private PlayerEmotionalState state;
    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform enemyArea;
    [SerializeField] private int baseHealth;
    [SerializeField] private int traumatizedBonusHealth;

    public PlayerEmotionalState State => state;
    public GameObject Prefab => prefab;
    public Transform EnemyArea => enemyArea;
    public int BaseHealth => Mathf.Max(1, baseHealth);
    public int TraumatizedBonusHealth => Mathf.Max(0, traumatizedBonusHealth);

    public int GetMaxHealth(bool isTraumatized)
    {
        return BaseHealth + (isTraumatized ? TraumatizedBonusHealth : 0);
    }
}