using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public int slotIndex;

    public string sceneName;
    public float posX;
    public float posY;

    public int hp;
    public int maxHp;
    public int level;

    public float playTime;

    public List<string> storyFlags = new();
    public List<string> visitedScenes = new();

    public bool HasStoryFlag(string flag)
    {
        return storyFlags.Contains(flag);
    }

    public void AddStoryFlag(string flag)
    {
        if (!storyFlags.Contains(flag))
        {
            storyFlags.Add(flag);
        }
    }

    public void MarkSceneVisited(string scene)
    {
        if (!visitedScenes.Contains(scene))
        {
            visitedScenes.Add(scene);
        }
    }
}
