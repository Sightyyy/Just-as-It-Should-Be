using System.IO;
using UnityEngine;

public static class SaveManager
{
    static string GetPath(int slot) =>
        Application.persistentDataPath + $"/save_{slot}.json";

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
}
