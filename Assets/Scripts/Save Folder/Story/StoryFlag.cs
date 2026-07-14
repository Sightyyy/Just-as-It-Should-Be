using UnityEngine;

public class StoryFlag : MonoBehaviour
{
    [SerializeField] private string flagKey;

    public string FlagKey => flagKey;

    public void ApplyTo(SaveData saveData)
    {
        if (saveData == null || string.IsNullOrWhiteSpace(flagKey)) return;

        saveData.AddStoryFlag(flagKey);
    }
}
