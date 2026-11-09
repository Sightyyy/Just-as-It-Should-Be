using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class FuzzyCleaningChallenge : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private Slider progressSlider;

    public bool IsActive { get; private set; }

    void Start()
    {
        Cancel();
    }

    public void Begin(KeyCode holdKey)
    {
        IsActive = true;
        SetPanel(true);
        SetProgress(0f, holdKey);
    }

    public void SetProgress(float normalized, KeyCode holdKey)
    {
        if (progressSlider != null)
        {
            progressSlider.value = Mathf.Clamp01(normalized);
        }

        if (promptText != null)
        {
            promptText.text = $"Hold {holdKey}";
        }
    }

    public void Finish()
    {
        IsActive = false;
        SetPanel(false);
    }

    public void Cancel()
    {
        IsActive = false;
        SetPanel(false);
    }

    private void SetPanel(bool active)
    {
        if (panel != null)
        {
            panel.SetActive(active);
        }
    }
}
