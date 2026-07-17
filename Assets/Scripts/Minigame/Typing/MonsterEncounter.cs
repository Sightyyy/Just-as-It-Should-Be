using UnityEngine;

public sealed class MonsterEncounter
{
    public PlayerEmotionalState State { get; }
    public TypingMonster Monster { get; }
    public GameObject SpawnedObject { get; }
    public Transform HealthBarAnchor { get; }

    public MonsterEncounter(PlayerEmotionalState state, TypingMonster monster, GameObject spawnedObject, Transform healthBarAnchor)
    {
        State = state;
        Monster = monster;
        SpawnedObject = spawnedObject;
        HealthBarAnchor = healthBarAnchor;
    }
}