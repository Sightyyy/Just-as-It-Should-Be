using UnityEngine;

public sealed class TypingInteractable : MonoBehaviour
{
    [SerializeField] private TypingMiniGameController typingMiniGame;
    [SerializeField] private GameObject interactPrompt;

    private bool playerInRange;

    void Awake()
    {
        typingMiniGame ??= FindAnyObjectByType<TypingMiniGameController>();
        //SetPrompt(false);
    }

    void Update()
    {
        if (GameplayInputLock.IsLocked || PatchingUpMinigameController.IsAnyPatchingActive)
        {
            //SetPrompt(false);
            return;
        }

        if (UiFocus.HasMenuSelection)
        {
            return;
        }

        if (TypingMiniGameController.IsAnyMinigameActive)
        {
            //SetPrompt(false);
            return;
        }

        bool canInteract = playerInRange && typingMiniGame != null && typingMiniGame.CanInteract();
        //SetPrompt(canInteract);

        if (canInteract && InteractionInput.TryConsumeInteraction())
        {
            typingMiniGame.Interact();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("player in range");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            //SetPrompt(false);
        }
    }

    //private void SetPrompt(bool active)
    //{
    //    if (interactPrompt != null)
    //    {
    //        interactPrompt.SetActive(active);
    //    }
    //}
}