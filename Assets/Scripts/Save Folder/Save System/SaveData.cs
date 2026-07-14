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

    public int calm;
    public int fear;
    public int doubt;
    public int courage;

    public List<string> storyFlags = new();
    public List<string> visitedScenes = new();
}
