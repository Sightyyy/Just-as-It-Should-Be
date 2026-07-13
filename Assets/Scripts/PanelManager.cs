using UnityEngine;

public class PanelManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject mapPanel;
    [SerializeField] private GameObject skillPanel;
    [SerializeField] private GameObject questListPanel;

    [Header("World Controls")]
    [SerializeField] private PlayerMovement realPlayer;
    [SerializeField] private PlayerMovement innerPlayer;

    private bool isPaused;

    void Start()
    {
        SetPanelActive(pausePanel, false);
        SetPanelActive(mapPanel, false);
        SetPanelActive(skillPanel, false);
        SetPanelActive(questListPanel, false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            TogglePanel(mapPanel);
        }

        if (Input.GetKeyDown(KeyCode.P) && IsControllingInnerSelf())
        {
            TogglePanel(skillPanel);
        }

        if (Input.GetKeyDown(KeyCode.J) && IsControllingRealLife())
        {
            TogglePanel(questListPanel);
        }

        CloseWorldLockedPanels();
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        SetPanelActive(pausePanel, isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void ResumeGame()
    {
        isPaused = false;
        SetPanelActive(pausePanel, false);
        Time.timeScale = 1f;
    }

    public void ToggleMap()
    {
        TogglePanel(mapPanel);
    }

    public void ToggleSkill()
    {
        if (!IsControllingInnerSelf()) return;

        TogglePanel(skillPanel);
    }

    public void ToggleQuestList()
    {
        if (!IsControllingRealLife()) return;

        TogglePanel(questListPanel);
    }

    private bool IsControllingRealLife()
    {
        return realPlayer != null && realPlayer.CanControl;
    }

    private bool IsControllingInnerSelf()
    {
        return innerPlayer != null && innerPlayer.CanControl;
    }

    private void CloseWorldLockedPanels()
    {
        if (!IsControllingRealLife())
        {
            SetPanelActive(questListPanel, false);
        }

        if (!IsControllingInnerSelf())
        {
            SetPanelActive(skillPanel, false);
        }
    }

    private void TogglePanel(GameObject panel)
    {
        if (panel == null) return;

        panel.SetActive(!panel.activeSelf);
    }

    private void SetPanelActive(GameObject panel, bool active)
    {
        if (panel == null) return;

        panel.SetActive(active);
    }

    void OnDestroy()
    {
        if (isPaused)
        {
            Time.timeScale = 1f;
        }
    }
}
