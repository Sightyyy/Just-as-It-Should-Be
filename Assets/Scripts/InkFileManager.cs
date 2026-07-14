using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InkFileManager", menuName = "Dialogue/Ink File Manager")]
public class InkFileManager : ScriptableObject
{
    [System.Serializable]
    public class ConversationBranch
    {
        public string conversationKey;
        public TextAsset inkFile;
        public List<string> nextPossibleBranches;
        [Tooltip("priority is to choose a conversation priority when multiple conversations is possible")]
        public int priority;
        [Tooltip("conditions on how to make this particular conversation happen")]
        public List<DialogueConversationCondition> conditions;
        public string completionFlag;
    }

    [SerializeField] private List<ConversationBranch> conversationBranches;
    private string currentConversationKey;

    public void StartConversation(string conversationKey)
    {
        if (!TryGetInkFileByKey(conversationKey, out _))
        {
            Debug.LogError($"Conversation branch not found or has no Ink file: {conversationKey}");
            return;
        }

        currentConversationKey = conversationKey;
    }

    public TextAsset GetCurrentInkFile()
    {
        return GetInkFileByKey(currentConversationKey);
    }

    public TextAsset GetInkFileByKey(string conversationKey)
    {
        ConversationBranch branch = conversationBranches.Find(b => b.conversationKey == conversationKey);
        return branch?.inkFile;
    }

    public bool TryGetInkFileByKey(string conversationKey, out TextAsset inkFile)
    {
        inkFile = GetInkFileByKey(conversationKey);
        return inkFile != null;
    }

    public bool TryGetConversation(string conversationKey, DialogueProgress progress, out ConversationBranch conversation)
    {
        conversation = conversationBranches.Find(branch => branch.conversationKey == conversationKey);
        return conversation != null && conversation.inkFile != null && IsEligible(conversation, progress);
    }

    public bool TryGetBestConversation(IReadOnlyList<string> candidateKeys, DialogueProgress progress, out ConversationBranch conversation)
    {
        conversation = null;
        if (conversationBranches == null)
        {
            return false;
        }

        foreach (ConversationBranch candidate in conversationBranches)
        {
            if (candidate?.inkFile == null || !IsCandidate(candidate.conversationKey, candidateKeys) || !IsEligible(candidate, progress))
            {
                continue;
            }

            if (conversation == null || candidate.priority > conversation.priority)
            {
                conversation = candidate;
            }
        }

        return conversation != null;
    }

    private static bool IsCandidate(string conversationKey, IReadOnlyList<string> candidateKeys)
    {
        if (candidateKeys == null || candidateKeys.Count == 0)
        {
            return true;
        }

        for (int index = 0; index < candidateKeys.Count; index++)
        {
            if (candidateKeys[index] == conversationKey)
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsEligible(ConversationBranch conversation, DialogueProgress progress)
    {
        if (conversation.conditions == null)
        {
            return true;
        }

        foreach (DialogueConversationCondition condition in conversation.conditions)
        {
            if (condition != null && !condition.IsMet(progress))
            {
                return false;
            }
        }

        return true;
    }

    public List<string> GetNextPossibleBranches()
    {
        ConversationBranch currentBranch = conversationBranches.Find(b => b.conversationKey == currentConversationKey);
        return currentBranch?.nextPossibleBranches ?? new List<string>();
    }

    public void MoveToNextBranch(string nextBranchKey)
    {
        if (conversationBranches.Exists(b => b.conversationKey == nextBranchKey))
        {
            Debug.Log("Moved to branch " +  nextBranchKey);
            currentConversationKey = nextBranchKey;
        }
        else
        {
            Debug.LogError($"Conversation branch not found: {nextBranchKey}");
        }
    }

    public bool HasNextBranches()
    {
        return GetNextPossibleBranches().Count > 0;
    }

    public bool IsDayTransitionBranch(string branchKey)
    {
        return branchKey == "next_day";
    }

    public bool IsToBeContinuedBranch(string branchKey)
    {
        return branchKey == "to_be_continued";
    }
}
