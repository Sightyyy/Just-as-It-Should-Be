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

    private void Update()
    {
        if (dialogueManager == null || player == null || dialogueManager.DialogueIsPlaying)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.T) && Vector2.Distance(player.position, transform.position) <= interactionRange)
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
