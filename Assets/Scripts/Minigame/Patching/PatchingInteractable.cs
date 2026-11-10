using UnityEngine;

public sealed class PatchingInteractable : MonoBehaviour
{
    [SerializeField] private PatchingUpMinigameController patchingMinigame;
    [SerializeField] private GameObject interactPrompt;

    private bool playerInRange;

    void Awake()
    {
        patchingMinigame ??= FindAnyObjectByType<PatchingUpMinigameController>();
        SetPrompt(false);
    }

    void Update()
    {
        if (GameplayInputLock.IsLocked || TypingMiniGameController.IsAnyMinigameActive)
        {
            if (patchingMinigame != null && patchingMinigame.IsPatchingActive)
            {
                patchingMinigame.StopPatching();
            }

            SetPrompt(false);
            return;
        }

        if (UiFocus.HasMenuSelection)
        {
            SetPrompt(false);
            return;
        }

        bool canInteract = playerInRange
            && patchingMinigame != null
            && patchingMinigame.CanBeginPatching();

        SetPrompt(canInteract || (patchingMinigame != null && patchingMinigame.IsPatchingActive));

        if (canInteract
            && !patchingMinigame.IsPatchingActive
            && InteractionInput.TryConsumeInteractionHold())
        {
            patchingMinigame.BeginPatching();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            SetPrompt(false);
            patchingMinigame?.StopPatching();
        }
    }

    private void SetPrompt(bool active)
    {
        if (interactPrompt != null)
        {
            interactPrompt.SetActive(active);
        }
    }
}
