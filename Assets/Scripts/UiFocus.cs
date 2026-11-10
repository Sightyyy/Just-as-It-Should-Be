using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class UiFocus
{
    public static bool HasMenuSelection
    {
        get
        {
            if (EventSystem.current == null) return false;

            GameObject selected = EventSystem.current.currentSelectedGameObject;
            return selected != null
                && selected.activeInHierarchy
                && selected.GetComponent<Selectable>() != null;
        }
    }
}
