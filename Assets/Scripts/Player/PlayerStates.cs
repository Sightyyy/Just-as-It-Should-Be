using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum PlayerEmotionalState
{
    Happy,
    Confident,
    Neutral,
    Afraid,
    Sad,
    Depressed,
    Traumatized
}

public sealed class PlayerStates : MonoBehaviour
{
    private readonly HashSet<PlayerEmotionalState> activeStates = new HashSet<PlayerEmotionalState>();

    public static PlayerStates Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("More than one PlayerStates was found. The duplicate was disabled.", this);
            enabled = false;
            return;
        }

        Instance = this;
    }

    private void OnEnable()
    {
        SaveManager.BeforeSave += WriteTo;
        SaveManager.AfterLoad += ReadFrom;
    }

    private void OnDisable()
    {
        SaveManager.BeforeSave -= WriteTo;
        SaveManager.AfterLoad -= ReadFrom;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public bool IsActive(PlayerEmotionalState state)
    {
        return activeStates.Contains(state);
    }

    public bool IsActive(string stateName)
    {
        return TryParse(stateName, out PlayerEmotionalState state) && IsActive(state);
    }

    public void SetActive(PlayerEmotionalState state, bool isActive)
    {
        if (isActive)
        {
            activeStates.Add(state);
        }
        else
        {
            activeStates.Remove(state);
        }
    }

    public void SetActive(string stateName, bool isActive)
    {
        if (TryParse(stateName, out PlayerEmotionalState state))
        {
            SetActive(state, isActive);
        }
        else
        {
            Debug.LogWarning($"Unknown player state '{stateName}'.");
        }
    }

    public void Activate(string stateName) => SetActive(stateName, true);

    public void Deactivate(string stateName) => SetActive(stateName, false);

    public IReadOnlyCollection<PlayerEmotionalState> GetActiveStates()
    {
        return activeStates;
    }

    public void WriteTo(SaveData data)
    {
        if (data == null)
        {
            return;
        }

        data.activeStates = activeStates.Select(state => state.ToString()).ToList();
    }

    public void ReadFrom(SaveData data)
    {
        if (data == null)
        {
            return;
        }

        activeStates.Clear();

        if (data.activeStates == null)
        {
            return;
        }

        foreach (string stateName in data.activeStates)
        {
            if (TryParse(stateName, out PlayerEmotionalState state))
            {
                activeStates.Add(state);
            }
        }
    }

    private static bool TryParse(string stateName, out PlayerEmotionalState state)
    {
        return Enum.TryParse(stateName?.Trim(), true, out state);
    }
}