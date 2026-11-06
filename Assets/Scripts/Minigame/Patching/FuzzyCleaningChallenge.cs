using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class FuzzyCleaningChallenge : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private KeyCode requiredKey = KeyCode.RightArrow;
    [SerializeField] private int requiredPresses = 5;

    private int currentPresses;
    private PatchingWound targetWound;
    private bool isActive;

    public bool IsActive => isActive;

    void Start()
    {
        SetPanel(false);
        RefreshUI();
    }

    void Update()
    {
        if (!isActive) return;

        if (Input.GetKeyDown(requiredKey))
        {
            currentPresses++;
            RefreshUI();

            if (currentPresses >= requiredPresses)
            {
                Complete();
            }
        }
    }

    public void Begin(PatchingWound wound)
    {
        if (wound == null || !wound.HasFuzzy) return;

        targetWound = wound;
        currentPresses = 0;
        isActive = true;
        SetPanel(true);
        RefreshUI();
    }

    public void Cancel()
    {
        targetWound = null;
        currentPresses = 0;
        isActive = false;
        SetPanel(false);
    }

    private void Complete()
    {
        targetWound?.CleanFuzzy();
        Cancel();
    }

    private void RefreshUI()
    {
        if (promptText != null)
        {
            promptText.text = $"{requiredKey} {currentPresses}/{Mathf.Max(1, requiredPresses)}";
        }

        if (progressSlider != null)
        {
            progressSlider.value = (float)currentPresses / Mathf.Max(1, requiredPresses);
        }
    }

    private void SetPanel(bool active)
    {
        if (panel != null)
        {
            panel.SetActive(active);
        }
    }
}
