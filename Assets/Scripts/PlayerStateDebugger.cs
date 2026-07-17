using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class PlayerStateDebugger : MonoBehaviour
{
    [Serializable]
    public struct StateKeyBinding
    {
        public PlayerEmotionalState state;
        public KeyCode key;
    }

    [SerializeField] private PlayerStates playerStates;
    [SerializeField] private List<StateKeyBinding> stateKeyBindings = new List<StateKeyBinding>();
    [SerializeField] private KeyCode resetAllKey = KeyCode.Backspace;
    [SerializeField] private bool logToConsole = true;
    [SerializeField] private bool showOnScreenDebugList = true;

    void Reset()
    {
        stateKeyBindings = new List<StateKeyBinding>
        {
            new StateKeyBinding { state = PlayerEmotionalState.Happy,       key = KeyCode.H },
            new StateKeyBinding { state = PlayerEmotionalState.Confident,   key = KeyCode.C },
            new StateKeyBinding { state = PlayerEmotionalState.Neutral,     key = KeyCode.N },
            new StateKeyBinding { state = PlayerEmotionalState.Afraid,      key = KeyCode.J },
            new StateKeyBinding { state = PlayerEmotionalState.Sad,         key = KeyCode.P },
            new StateKeyBinding { state = PlayerEmotionalState.Depressed,   key = KeyCode.D },
            new StateKeyBinding { state = PlayerEmotionalState.Traumatized, key = KeyCode.T },
        };
    }

    void Awake()
    {
        playerStates ??= PlayerStates.Instance;
    }

    void Update()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (TypingMiniGameController.IsAnyMinigameActive) return;

        if (playerStates == null)
        {
            playerStates = PlayerStates.Instance;
            if (playerStates == null) return;
        }

        foreach (StateKeyBinding binding in stateKeyBindings)
        {
            if (Input.GetKeyDown(binding.key))
            {
                ToggleState(binding.state);
            }
        }

        if (Input.GetKeyDown(resetAllKey))
        {
            ResetAllStates();
        }
#endif
    }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
    void OnGUI()
    {
        if (!showOnScreenDebugList || playerStates == null) return;

        GUILayout.BeginArea(new Rect(10, 10, 260, 260), GUI.skin.box);
        GUILayout.Label("<b>Player State Debugger</b>", new GUIStyle(GUI.skin.label) { richText = true });

        foreach (StateKeyBinding binding in stateKeyBindings)
        {
            bool isActive = playerStates.IsActive(binding.state);
            GUILayout.Label($"[{binding.key}] {binding.state}: {(isActive ? "ON" : "off")}");
        }

        GUILayout.Space(6);
        GUILayout.Label($"[{resetAllKey}] Reset all states");
        GUILayout.EndArea();
    }
#endif

    private void ToggleState(PlayerEmotionalState state)
    {
        bool newValue = !playerStates.IsActive(state);
        playerStates.SetActive(state, newValue);

        if (logToConsole)
        {
            Debug.Log($"[PlayerStateDebugger] {state} => {(newValue ? "ON" : "OFF")}");
        }
    }

    private void ResetAllStates()
    {
        foreach (PlayerEmotionalState state in (PlayerEmotionalState[])Enum.GetValues(typeof(PlayerEmotionalState)))
        {
            playerStates.SetActive(state, false);
        }

        if (logToConsole)
        {
            Debug.Log("[PlayerStateDebugger] All states reset - monsters should despawn.");
        }
    }
}