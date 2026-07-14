using UnityEngine;

[System.Serializable]
public sealed class DialogueConversationCondition
{
    public DialogueConditionType type;
    public string statName;
    public int value;
    public string flag;

    public bool IsMet(DialogueProgress progress)
    {
        if (progress == null)
        {
            return type == DialogueConditionType.None;
        }

        switch (type)
        {
            case DialogueConditionType.None:
                return true;
            case DialogueConditionType.StatAtLeast:
                return progress.GetStat(statName) >= value;
            case DialogueConditionType.StatAtMost:
                return progress.GetStat(statName) <= value;
            case DialogueConditionType.HasFlag:
                return progress.HasFlag(flag);
            case DialogueConditionType.MissingFlag:
                return !progress.HasFlag(flag);
            default:
                Debug.LogWarning($"Unknown dialogue condition type '{type}'.");
                return false;
        }
    }
}

public enum DialogueConditionType
{
    None,
    StatAtLeast,
    StatAtMost,
    HasFlag,
    MissingFlag
}
