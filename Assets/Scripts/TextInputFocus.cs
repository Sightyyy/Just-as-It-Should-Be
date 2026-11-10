using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public static class TextInputFocus
{
    public static bool IsFocused
    {
        get
        {
            GameObject selected = EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null;
            return selected != null && selected.GetComponent<TMP_InputField>() != null;
        }
    }
}
