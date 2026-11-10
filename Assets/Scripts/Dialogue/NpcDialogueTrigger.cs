using System.Collections.Generic;
using UnityEngine;

public sealed class NpcDialogueTrigger : MonoBehaviour
{
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private Transform player;
    [Tooltip("Put the conversationKey in this list if there are more than 1 possible conversation that might happen")]
    [SerializeField] private List<string> possibleConversationKeys = new List<string>();
    [Tooltip("If only 1 conversation in this npc, just put it here")]
    [SerializeField] private string defaultConversationKey = "First_Conversation";
    [SerializeField, Min(0.1f)] private float interactionRange = 2f;

    private PlayerMovement playerMovement;

    private void Update()
    {
        if (GameplayInputLock.IsLocked) return;
        if (TypingMiniGameController.IsAnyMinigameActive) return;
        if (PatchingUpMinigameController.IsAnyPatchingActive) return;
        if (UiFocus.HasMenuSelection) return;

        if (dialogueManager == null || player == null || dialogueManager.DialogueIsPlaying)
        {
            return;
        }

        playerMovement ??= player.GetComponent<PlayerMovement>();
        if (playerMovement != null && !playerMovement.CanControl) return;

        if (Vector2.Distance(player.position, transform.position) <= interactionRange && InteractionInput.TryConsumeInteraction())
        {
            if (possibleConversationKeys.Count > 0)
            {
                dialogueManager.StartBestConversation(possibleConversationKeys);
            }
            else
            {
                dialogueManager.StartConversation(defaultConversationKey);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}