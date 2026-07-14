using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class DialogueProgress : MonoBehaviour
{
    private readonly HashSet<string> flags = new HashSet<string>(StringComparer.Ordinal);

    public static DialogueProgress Instance { get; private set; }

    public int Calm { get; private set; }
    public int Fear { get; private set; }
    public int Doubt { get; private set; }
    public int Courage { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("More than one DialogueProgress was found. The duplicate was disabled.", this);
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

    public int GetStat(string statName)
    {
        switch (Normalize(statName))
        {
            case "calm": return Calm;
            case "fear": return Fear;
            case "doubt": return Doubt;
            case "courage": return Courage;
            default:
                Debug.LogWarning($"Unknown dialogue stat '{statName}'.", this);
                return 0;
        }
    }

    public void SetStat(string statName, int value)
    {
        switch (Normalize(statName))
        {
            case "calm": Calm = value; break;
            case "fear": Fear = value; break;
            case "doubt": Doubt = value; break;
            case "courage": Courage = value; break;
            default: Debug.LogWarning($"Unknown dialogue stat '{statName}'.", this); break;
        }
    }

    public void AddStat(string statName, int amount)
    {
        SetStat(statName, GetStat(statName) + amount);
    }

    public bool HasFlag(string flag)
    {
        return !string.IsNullOrWhiteSpace(flag) && flags.Contains(flag.Trim());
    }

    public void SetFlag(string flag)
    {
        if (!string.IsNullOrWhiteSpace(flag))
        {
            flags.Add(flag.Trim());
        }
    }

    public void WriteTo(SaveData data)
    {
        if (data == null)
        {
            return;
        }

        data.calm = Calm;
        data.fear = Fear;
        data.doubt = Doubt;
        data.courage = Courage;
        data.storyFlags ??= new List<string>();

        foreach (string flag in flags)
        {
            if (!data.storyFlags.Contains(flag))
            {
                data.storyFlags.Add(flag);
            }
        }
    }

    public void ReadFrom(SaveData data)
    {
        if (data == null)
        {
            return;
        }

        Calm = data.calm;
        Fear = data.fear;
        Doubt = data.doubt;
        Courage = data.courage;
        flags.Clear();

        if (data.storyFlags == null)
        {
            return;
        }

        foreach (string flag in data.storyFlags)
        {
            SetFlag(flag);
        }
    }

    private static string Normalize(string value)
    {
        return value?.Trim().ToLowerInvariant() ?? string.Empty;
    }
}
