using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

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
    [SerializeField] private AnalogClockHand analogClock;

    private bool consoleOpen;
    private bool helpShown;
    private bool unlockNextFrame;
    private bool focusInputNextFrame;
    private int lastSubmitFrame = -1;

    public bool ConsoleOpen => consoleOpen;

    void Awake()
    {
        playerStates ??= PlayerStates.Instance;
        stateDebugger ??= FindAnyObjectByType<PlayerStateDebugger>();
        patchingMinigame ??= FindAnyObjectByType<PatchingUpMinigameController>();
        skillUpgradeManager ??= SkillUpgradeManager.Instance;
        analogClock ??= FindAnyObjectByType<AnalogClockHand>();

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

        stateDebugger?.SetOverlaySuppressed(false);
        GameplayInputLock.Unlock(this);
    }

    void Update()
    {
        if (consoleOpen)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SetConsoleOpen(false);
            }

            return;
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (IsConsoleShortcutPressed() && CanOpenConsole())
        {
            SetConsoleOpen(true);
        }
#endif
    }

    // Unlocking here (instead of inside Update) keeps the pause menu from reacting
    // to the same Escape press that closed the console.
    void LateUpdate()
    {
        if (focusInputNextFrame)
        {
            focusInputNextFrame = false;

            if (consoleOpen)
            {
                FocusInput();
            }
        }

        if (!unlockNextFrame) return;

        unlockNextFrame = false;
        GameplayInputLock.Unlock(this);
    }

    private void ExecuteCommand(string rawCommand)
    {
        if (!consoleOpen) return;

        // TMP raises onSubmit twice for one Enter: once from its own key handling
        // and once from the EventSystem submit action.
        if (Time.frameCount == lastSubmitFrame) return;
        lastSubmitFrame = Time.frameCount;

        string command = (rawCommand ?? string.Empty).Trim();
        if (command.Length == 0 || command == "/")
        {
            ResetInputField();
            focusInputNextFrame = true;
            return;
        }

        AppendOutput($"> {command}");

        command = command.TrimStart('/').Trim();

        string[] parts = command.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length > 0)
        {
            HandleCommand(parts);
        }

        ResetInputField();
        focusInputNextFrame = true;
    }

    private void HandleCommand(string[] parts)
    {
        string name = parts[0].ToLowerInvariant();

        switch (name)
        {
            case "state":
                SetState(parts);
                break;
            case "states":
                ListStates();
                break;
            case "resetstates":
                ResetStates();
                break;
            case "wound":
                SpawnWound(parts);
                break;
            case "upgrade":
                UpgradeSkill(parts);
                break;
            case "time":
                HandleTime(parts);
                break;
            case "help":
                ShowHelp();
                break;
            default:
                AppendOutput($"Unknown command: {parts[0]}. Type /help.");
                break;
        }
    }

    private void SetState(string[] parts)
    {
        if (parts.Length < 2)
        {
            AppendOutput("Usage: /state <name> [on|off]");
            return;
        }

        if (!Enum.TryParse(parts[1], true, out PlayerEmotionalState state))
        {
            AppendOutput($"Unknown state: {parts[1]}");
            AppendOutput("States: " + string.Join(", ", Enum.GetNames(typeof(PlayerEmotionalState))));
            return;
        }

        bool enable = true;
        bool replaceAll = parts.Length < 3;
        if (parts.Length >= 3)
        {
            if (IsOnValue(parts[2]))
            {
                enable = true;
            }
            else if (IsOffValue(parts[2]))
            {
                enable = false;
            }
            else
            {
                AppendOutput("Usage: /state <name> [on|off]");
                return;
            }
        }

        playerStates ??= PlayerStates.Instance;
        if (playerStates == null)
        {
            AppendOutput("No PlayerStates found.");
            return;
        }

        if (replaceAll)
        {
            playerStates.ClearAll();
        }

        playerStates.SetActive(state, enable);
        AppendOutput($"{state}: {(enable ? "ON" : "off")}");
    }

    private void ListStates()
    {
        playerStates ??= PlayerStates.Instance;
        if (playerStates == null)
        {
            AppendOutput("No PlayerStates found.");
            return;
        }

        var activeStates = playerStates.GetActiveStates();
        AppendOutput(activeStates.Count == 0
            ? "No active states."
            : "Active states: " + string.Join(", ", activeStates));
    }

    private void ResetStates()
    {
        if (stateDebugger != null)
        {
            stateDebugger.ResetAllStates();
        }
        else
        {
            playerStates ??= PlayerStates.Instance;
            playerStates?.ClearAll();
        }

        AppendOutput("All player states reset.");
    }

    private void SpawnWound(string[] parts)
    {
        bool hasFuzzy = false;
        if (parts.Length >= 2)
        {
            if (parts[1].Equals("fuzzy", StringComparison.OrdinalIgnoreCase))
            {
                hasFuzzy = true;
            }
            else if (!parts[1].Equals("clean", StringComparison.OrdinalIgnoreCase))
            {
                AppendOutput("Usage: /wound [fuzzy|clean]");
                return;
            }
        }

        patchingMinigame ??= FindAnyObjectByType<PatchingUpMinigameController>();
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

        skillUpgradeManager ??= SkillUpgradeManager.Instance;
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
        AppendOutput(upgraded
            ? $"Upgraded {type} to level {skillUpgradeManager.GetLevel(type)}."
            : $"{type} is already maxed at level {skillUpgradeManager.GetMaxLevel(type)}.");
    }

    private void HandleTime(string[] parts)
    {
        analogClock ??= FindAnyObjectByType<AnalogClockHand>();
        if (analogClock == null)
        {
            AppendOutput("No AnalogClockHand found.");
            return;
        }

        if (parts.Length == 1)
        {
            AppendOutput($"Time is {analogClock.CurrentTimeText}.");
            return;
        }

        if (parts[1].Equals("add", StringComparison.OrdinalIgnoreCase))
        {
            if (parts.Length < 3 || !float.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out float hours))
            {
                AppendOutput("Usage: /time add <hours>");
                return;
            }

            analogClock.AddGameHours(hours);
            AppendOutput($"Time is now {analogClock.CurrentTimeText}.");
            return;
        }

        if (parts[1].Equals("set", StringComparison.OrdinalIgnoreCase))
        {
            if (parts.Length < 3 || !TryParseTime(parts[2], out int setHour, out int setMinute))
            {
                AppendOutput("Usage: /time set HH:MM");
                return;
            }

            analogClock.SetTime(setHour, setMinute);
            AppendOutput($"Time set to {analogClock.CurrentTimeText}.");
            return;
        }

        if (TryParseTime(parts[1], out int hour, out int minute))
        {
            analogClock.SetTime(hour, minute);
            AppendOutput($"Time set to {analogClock.CurrentTimeText}.");
            return;
        }

        AppendOutput("Usage: /time, /time add <hours>, /time set HH:MM");
    }

    private void ShowHelp()
    {
        AppendOutput("Commands:");
        AppendOutput("/state <name> [on|off] - become <name>; on/off toggles it");
        AppendOutput("/states - list active states");
        AppendOutput("/resetstates - clear all states");
        AppendOutput("/wound [fuzzy|clean] - spawn a patching wound");
        AppendOutput("/upgrade damage|fortification|clearmind - grant an upgrade");
        AppendOutput("/time - show clock time");
        AppendOutput("/time add <hours> - advance the clock");
        AppendOutput("/time set HH:MM - set the clock");
        AppendOutput("/help - show this list");
    }

    private void SetConsoleOpen(bool open)
    {
        bool wasOpen = consoleOpen;
        consoleOpen = open;

        if (consolePanel != null)
        {
            consolePanel.SetActive(open);
        }

        stateDebugger?.SetOverlaySuppressed(open);

        if (open)
        {
            unlockNextFrame = false;
            GameplayInputLock.Lock(this);

            if (!helpShown)
            {
                helpShown = true;
                AppendOutput("Type /help for a list of commands.");
            }

            ResetInputField();
            focusInputNextFrame = true;
        }
        else if (wasOpen)
        {
            unlockNextFrame = true;
            focusInputNextFrame = false;

            if (EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
    }

    private void ResetInputField()
    {
        if (commandInput == null) return;

        commandInput.text = "/";
        commandInput.caretPosition = commandInput.text.Length;
    }

    private void FocusInput()
    {
        if (commandInput == null) return;

        commandInput.ActivateInputField();
        commandInput.caretPosition = commandInput.text.Length;
    }

    private void AppendOutput(string line)
    {
        if (outputText != null)
        {
            outputText.text = string.IsNullOrEmpty(outputText.text) ? line : $"{outputText.text}\n{line}";
        }

        Debug.Log($"[DevCommandConsole] {line}");
    }

    private static bool IsOnValue(string value)
    {
        return value.Equals("on", StringComparison.OrdinalIgnoreCase)
            || value.Equals("true", StringComparison.OrdinalIgnoreCase)
            || value == "1";
    }

    private static bool IsOffValue(string value)
    {
        return value.Equals("off", StringComparison.OrdinalIgnoreCase)
            || value.Equals("false", StringComparison.OrdinalIgnoreCase)
            || value == "0";
    }

    private static bool TryParseTime(string value, out int hour, out int minute)
    {
        hour = 0;
        minute = 0;

        string[] parts = value.Split(':');
        if (parts.Length != 2) return false;

        if (!int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out hour)) return false;
        if (!int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out minute)) return false;

        return hour >= 0 && hour < 24 && minute >= 0 && minute < 60;
    }

    private static bool CanOpenConsole()
    {
        bool dialoguePlaying = DialogueManager.Instance != null && DialogueManager.Instance.DialogueIsPlaying;

        return !dialoguePlaying
            && !UiFocus.HasMenuSelection
            && !TypingMiniGameController.IsAnyMinigameActive
            && !PatchingUpMinigameController.IsAnyPatchingActive;
    }

    private static bool IsConsoleShortcutPressed()
    {
        bool ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        bool shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        bool enter = Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter);
        return ctrl && shift && enter;
    }
}
