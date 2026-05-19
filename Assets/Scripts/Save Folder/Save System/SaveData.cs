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
}
