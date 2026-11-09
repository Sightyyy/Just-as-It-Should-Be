using UnityEngine;

public class PanelManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject mapPanel;
    [SerializeField] private MapController mapController;
    [SerializeField] private GameObject skillPanel;
    [SerializeField] private GameObject questListPanel;

    [Header("World Controls")]
    [SerializeField] private PlayerMovement realPlayer;
    [SerializeField] private PlayerMovement innerPlayer;

    private bool isPaused;
    private bool isSkillOpen;

    void Start()
    {
        SetPanelActive(pausePanel, false);
        SetMapOpen(false);
        SetSkillOpen(false);
        SetPanelActive(questListPanel, false);
    }

    void Update()
    {
        if (isSkillOpen)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.B))
            {
                SetSkillOpen(false);
            }

            CloseWorldLockedPanels();
            return;
        }

        if (PatchingUpMinigameController.IsAnyPatchingActive)
        {
            SetSkillOpen(false);
            SetPanelActive(questListPanel, false);
            return;
        }

        if (TypingMiniGameController.IsAnyMinigameActive || GameplayInputLock.IsLocked)
        {
            SetSkillOpen(false);
            SetPanelActive(questListPanel, false);
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        if ((Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.B)) && IsControllingInnerSelf())
        {
            SetSkillOpen(!isSkillOpen);
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
        if (mapController == null) return;

        mapController.ToggleMap();
    }

    private void SetMapOpen(bool open)
    {
        if (mapController == null) return;

        mapController.SetMapOpen(open);
    }

    public void ToggleSkill()
    {
        if (!IsControllingInnerSelf()) return;

        SetSkillOpen(!isSkillOpen);
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
            SetSkillOpen(false);
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

    private void SetSkillOpen(bool active)
    {
        isSkillOpen = active;
        SetPanelActive(skillPanel, active);

        if (active)
        {
            GameplayInputLock.Lock(this);
        }
        else
        {
            GameplayInputLock.Unlock(this);
        }
    }

    void OnDestroy()
    {
        if (isPaused)
        {
            Time.timeScale = 1f;
        }

        GameplayInputLock.Unlock(this);
    }
}
