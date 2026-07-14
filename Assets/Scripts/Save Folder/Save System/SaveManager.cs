using System.IO;
using System;
using UnityEngine;

public static class SaveManager
{
    public static event Action<SaveData> BeforeSave;
    public static event Action<SaveData> AfterLoad;

    static string GetPath(int slot) =>
        Application.persistentDataPath + $"/save_{slot}.json";

    public static void Save(SaveData data)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));

        BeforeSave?.Invoke(data);
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetPath(data.slotIndex), json);
    }

    public static SaveData Load(int slot)
    {
        string path = GetPath(slot);
        if (!File.Exists(path)) return null;

        SaveData data = JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
        AfterLoad?.Invoke(data);
        return data;
    }

    private static string GetPath(int slot)
    {
        return Path.Combine(Application.persistentDataPath, $"{SaveFilePrefix}_{slot}.{SaveFileExtension}");
    }
}
