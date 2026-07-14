using UnityEngine;

[System.Serializable]
public sealed class DialogueConversationCondition
{
    public DialogueConditionType type;
    public string flag;
    public string stateName;

    public bool IsMet(DialogueProgress progress, PlayerStates states)
    {
        switch (type)
        {
            case DialogueConditionType.None:
                return true;
            case DialogueConditionType.HasFlag:
                return progress != null && progress.HasFlag(flag);
            case DialogueConditionType.MissingFlag:
                return progress == null || !progress.HasFlag(flag);
            case DialogueConditionType.HasState:
                return states != null && states.IsActive(stateName);
            case DialogueConditionType.MissingState:
                return states == null || !states.IsActive(stateName);
            default:
                Debug.LogWarning($"Unknown dialogue condition type '{type}'.");
                return false;
        }
    }
}

public enum DialogueConditionType
{
    None = 0,
    HasFlag = 3,
    MissingFlag = 4,
    HasState = 5,
    MissingState = 6
}