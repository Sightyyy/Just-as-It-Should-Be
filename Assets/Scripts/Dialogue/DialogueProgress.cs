using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class DialogueProgress : MonoBehaviour
{
    private readonly HashSet<string> flags = new HashSet<string>(StringComparer.Ordinal);

    public static DialogueProgress Instance { get; private set; }

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
}