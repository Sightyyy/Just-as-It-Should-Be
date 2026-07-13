using System.IO;
using UnityEngine;

public static class SaveManager
{
    private const string SaveFilePrefix = "save";
    private const string SaveFileExtension = "json";

    public static bool HasSave(int slot)
    {
        return File.Exists(GetPath(slot));
    }

    public static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetPath(data.slotIndex), json);
    }

    public static SaveData Load(int slot)
    {
        string path = GetPath(slot);
        if (!File.Exists(path)) return null;

        return JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
    }

    private static string GetPath(int slot)
    {
        return Path.Combine(Application.persistentDataPath, $"{SaveFilePrefix}_{slot}.{SaveFileExtension}");
    }
}
