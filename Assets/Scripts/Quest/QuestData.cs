using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "Quest/Quest Data")]
public class QuestData : ScriptableObject
{
    [Tooltip("Unique id. Flag names in DialogueProgress follow the pattern: accept = \"quest_<id>\", done = \"quest_<id>_done\".")]
    public string id;

    public string title;

    [TextArea(2, 5)]
    public string description;

    [Tooltip("DialogueProgress flag that makes this quest visible in the quest list.")]
    public string acceptFlag;

    [Tooltip("DialogueProgress flag that marks this quest as completed.")]
    public string completeFlag;

    public List<QuestObjective> objectives = new();

    [Serializable]
    public class QuestObjective
    {
        public string text;

        [Tooltip("DialogueProgress flag that marks this objective as done. Empty = follow the quest's complete flag.")]
        public string doneFlag;
    }
}
