using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[DefaultExecutionOrder(-40)]
public sealed class UiSelectionHighlighter : MonoBehaviour
{
    [SerializeField] private Color selectedTextColor = new Color(1f, 0.85f, 0.2f, 1f);

    private readonly List<TMP_Text> highlightedTexts = new List<TMP_Text>();
    private readonly List<Color> originalColors = new List<Color>();
    private GameObject highlightedObject;

    private void Update()
    {
        GameObject selected = EventSystem.current == null || TextInputFocus.IsFocused
            ? null
            : EventSystem.current.currentSelectedGameObject;

        if (selected == highlightedObject)
        {
            return;
        }

        RestoreColors();
        highlightedObject = selected;

        if (selected == null)
        {
            return;
        }

        foreach (TMP_Text text in selected.GetComponentsInChildren<TMP_Text>(true))
        {
            highlightedTexts.Add(text);
            originalColors.Add(text.color);
            text.color = selectedTextColor;
        }
    }

    private void OnDisable()
    {
        RestoreColors();
        highlightedObject = null;
    }

    private void RestoreColors()
    {
        for (int index = 0; index < highlightedTexts.Count; index++)
        {
            if (highlightedTexts[index] != null)
            {
                highlightedTexts[index].color = originalColors[index];
            }
        }

        highlightedTexts.Clear();
        originalColors.Clear();
    }
}
