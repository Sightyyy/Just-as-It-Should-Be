using System;
using TMPro;
using UnityEngine;

public sealed class DevCommandConsole : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject consolePanel;
    [SerializeField] private TMP_InputField commandInput;
    [SerializeField] private TextMeshProUGUI outputText;

    [Header("Dependencies")]
    [SerializeField] private PlayerStateDebugger stateDebugger;
    [SerializeField] private PlayerStates playerStates;
    [SerializeField] private PatchingUpMinigameController patchingMinigame;
    [SerializeField] private SkillUpgradeManager skillUpgradeManager;

    private bool devModeEnabled;
    private bool consoleOpen;

    public bool DevModeEnabled => devModeEnabled;
    public bool ConsoleOpen => consoleOpen;

    void Awake()
    {
        playerStates ??= PlayerStates.Instance;
        stateDebugger ??= FindAnyObjectByType<PlayerStateDebugger>();
        patchingMinigame ??= FindAnyObjectByType<PatchingUpMinigameController>();
        skillUpgradeManager ??= SkillUpgradeManager.Instance;

        if (commandInput != null)
        {
            commandInput.onSubmit.AddListener(ExecuteCommand);
        }

        SetConsoleOpen(false);
    }

    void OnDestroy()
    {
        if (commandInput != null)
        {
            commandInput.onSubmit.RemoveListener(ExecuteCommand);
        }

        GameplayInputLock.Unlock(this);
    }

    void Update()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (IsDevModeShortcutPressed())
        {
            devModeEnabled = !devModeEnabled;
            AppendOutput(devModeEnabled ? "Dev mode enabled." : "Dev mode disabled.");

            if (!devModeEnabled)
            {
                SetConsoleOpen(false);
            }
        }

        if (!devModeEnabled) return;

        if (!consoleOpen && Input.GetKeyDown(KeyCode.Slash))
        {
            SetConsoleOpen(true);
        }
        else if (consoleOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            SetConsoleOpen(false);
        }
#endif
    }

    private void ExecuteCommand(string rawCommand)
    {
        if (!consoleOpen || string.IsNullOrWhiteSpace(rawCommand)) return;

        string command = rawCommand.Trim();
        if (command.StartsWith("/"))
        {
            command = command.Substring(1);
        }

        string[] parts = command.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return;

        HandleCommand(parts);

        if (commandInput != null)
        {
            commandInput.text = string.Empty;
            commandInput.ActivateInputField();
            commandInput.Select();
        }
    }

    private void HandleCommand(string[] parts)
    {
        string name = parts[0].ToLowerInvariant();

        switch (name)
        {
            case "state":
                ToggleState(parts);
                break;
            case "resetstates":
                stateDebugger?.ResetAllStates();
                AppendOutput("All player states reset.");
                break;
            case "wound":
                SpawnWound(parts);
                break;
            case "upgrade":
                UpgradeSkill(parts);
                break;
            case "help":
                AppendOutput("Commands: /state afraid, /resetstates, /wound fuzzy, /wound clean, /upgrade damage|fortification|clearmind");
                break;
            default:
                AppendOutput($"Unknown command: {parts[0]}");
                break;
        }
    }

    private void ToggleState(string[] parts)
    {
        if (parts.Length < 2)
        {
            AppendOutput("Usage: /state <name>");
            return;
        }

        if (!Enum.TryParse(parts[1], true, out PlayerEmotionalState state))
        {
            AppendOutput($"Unknown state: {parts[1]}");
            return;
        }

        stateDebugger?.ToggleState(state);
        bool active = playerStates != null && playerStates.IsActive(state);
        AppendOutput($"{state}: {(active ? "ON" : "off")}");
    }

    private void SpawnWound(string[] parts)
    {
        bool hasFuzzy = parts.Length >= 2 && (parts[1].Equals("fuzzy", StringComparison.OrdinalIgnoreCase) || parts[1].Equals("true", StringComparison.OrdinalIgnoreCase));

        if (patchingMinigame == null)
        {
            AppendOutput("No PatchingUpMinigameController found.");
            return;
        }

        patchingMinigame.SpawnDebugWound(hasFuzzy);
        AppendOutput(hasFuzzy ? "Spawned wound with fuzzy." : "Spawned clean wound.");
    }

    private void UpgradeSkill(string[] parts)
    {
        if (parts.Length < 2)
        {
            AppendOutput("Usage: /upgrade damage|fortification|clearmind");
            return;
        }

        if (skillUpgradeManager == null)
        {
            skillUpgradeManager = SkillUpgradeManager.Instance;
        }

        if (skillUpgradeManager == null)
        {
            AppendOutput("No SkillUpgradeManager found.");
            return;
        }

        SkillUpgradeType type;
        string value = parts[1].ToLowerInvariant();
        if (value == "damage")
        {
            type = SkillUpgradeType.ConvincingDamage;
        }
        else if (value == "fortification")
        {
            type = SkillUpgradeType.FeelingsFortification;
        }
        else if (value == "clearmind")
        {
            type = SkillUpgradeType.ClearMind;
        }
        else
        {
            AppendOutput($"Unknown upgrade: {parts[1]}");
            return;
        }

        bool upgraded = skillUpgradeManager.Upgrade(type);
        AppendOutput(upgraded ? $"Upgraded {type}." : $"{type} is already maxed.");
    }

    private void SetConsoleOpen(bool open)
    {
        consoleOpen = open;

        if (consolePanel != null)
        {
            consolePanel.SetActive(open);
        }

        if (open)
        {
            GameplayInputLock.Lock(this);
            if (commandInput != null)
            {
                commandInput.text = "/";
                commandInput.ActivateInputField();
                commandInput.Select();
                commandInput.caretPosition = commandInput.text.Length;
            }
        }
        else
        {
            GameplayInputLock.Unlock(this);
        }
    }

    private void AppendOutput(string line)
    {
        if (outputText != null)
        {
            outputText.text = string.IsNullOrEmpty(outputText.text) ? line : $"{outputText.text}\n{line}";
        }

        Debug.Log($"[DevCommandConsole] {line}");
    }

    private static bool IsDevModeShortcutPressed()
    {
        bool alt = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
        bool ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        bool shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        return alt && ctrl && shift && Input.GetKeyDown(KeyCode.Backspace);
    }
}
