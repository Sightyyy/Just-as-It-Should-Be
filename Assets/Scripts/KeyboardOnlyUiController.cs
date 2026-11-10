using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

[DefaultExecutionOrder(-50)]
public sealed class KeyboardOnlyUiController : MonoBehaviour
{
    private InputSystemUIInputModule inputModule;

    private void Awake()
    {
        inputModule = GetComponent<InputSystemUIInputModule>();
        if (inputModule == null)
        {
            return;
        }

        inputModule.point = null;
        inputModule.leftClick = null;
        inputModule.rightClick = null;
        inputModule.middleClick = null;
        inputModule.scrollWheel = null;
    }

    private void Update()
    {
        if (EventSystem.current == null)
        {
            return;
        }

        EnsureSelection();
        TrySubmit();
    }

    private void EnsureSelection()
    {
        if (DialogueManager.Instance != null && DialogueManager.Instance.DialogueIsPlaying)
        {
            if (!TextInputFocus.IsFocused && EventSystem.current.currentSelectedGameObject != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }

            return;
        }

        GameObject selected = EventSystem.current.currentSelectedGameObject;
        if (IsSelectionUsable(selected))
        {
            return;
        }

        if (TextInputFocus.IsFocused)
        {
            return;
        }

        Selectable first = FindFirstSelectable();
        if (first != null)
        {
            EventSystem.current.SetSelectedGameObject(first.gameObject);
        }
    }

    private void TrySubmit()
    {
        if (TextInputFocus.IsFocused)
        {
            return;
        }

        if (DialogueManager.Instance != null && DialogueManager.Instance.DialogueIsPlaying)
        {
            return;
        }

        if (TypingMiniGameController.IsAnyMinigameActive || PatchingUpMinigameController.IsAnyPatchingActive)
        {
            return;
        }

        GameObject selected = EventSystem.current.currentSelectedGameObject;
        if (selected == null)
        {
            return;
        }

        GameObject submitTarget = ExecuteEvents.GetEventHandler<ISubmitHandler>(selected);
        if (submitTarget == null || !InteractionInput.TryConsumeInteraction())
        {
            return;
        }

        ExecuteEvents.Execute(submitTarget, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
    }

    private static bool IsSelectionUsable(GameObject selected)
    {
        if (selected == null || !selected.activeInHierarchy)
        {
            return false;
        }

        Selectable selectable = selected.GetComponent<Selectable>();
        return selectable == null || selectable.IsInteractable();
    }

    private static Selectable FindFirstSelectable()
    {
        Selectable[] selectables = FindObjectsByType<Selectable>();
        Selectable best = null;
        Vector3 bestPosition = default;

        foreach (Selectable selectable in selectables)
        {
            if (selectable == null || !selectable.isActiveAndEnabled || !selectable.IsInteractable())
            {
                continue;
            }

            if (!(selectable.transform is RectTransform rect))
            {
                continue;
            }

            Vector3 position = rect.position;
            bool isHigher = best == null || position.y > bestPosition.y + 0.01f;
            bool isSameRowAndLeft = best != null && Mathf.Abs(position.y - bestPosition.y) <= 0.01f && position.x < bestPosition.x;
            if (isHigher || isSameRowAndLeft)
            {
                best = selectable;
                bestPosition = position;
            }
        }

        return best;
    }
}
