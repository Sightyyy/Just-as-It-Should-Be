using UnityEngine;

public sealed class PatchingInteractable : MonoBehaviour
{
    [SerializeField] private PatchingUpMinigameController patchingMinigame;
    [SerializeField] private PlayerStates playerStates;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private GameObject interactPrompt;

    private bool playerInRange;

    void Awake()
    {
        patchingMinigame ??= FindAnyObjectByType<PatchingUpMinigameController>();
        playerStates ??= PlayerStates.Instance;
        SetPrompt(false);
    }

    void Update()
    {
        bool canInteract = playerInRange && CanInteractWithCurrentState();
        SetPrompt(canInteract);

        if (canInteract && Input.GetKeyDown(interactKey))
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

    private bool CanInteractWithCurrentState()
    {
        if (playerStates == null)
        {
            playerStates = PlayerStates.Instance;
        }

        return playerStates != null
            && patchingMinigame != null
            && patchingMinigame.CanBeginPatching()
            && (playerStates.IsActive(PlayerEmotionalState.Afraid)
                || playerStates.IsActive(PlayerEmotionalState.Sad)
                || playerStates.IsActive(PlayerEmotionalState.Depressed));
    }

    private void SetPrompt(bool active)
    {
        if (interactPrompt != null)
        {
            interactPrompt.SetActive(active);
        }
    }
}
