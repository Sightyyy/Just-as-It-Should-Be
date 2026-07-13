using UnityEngine;
using TMPro;

public class SaveSlotInfo : MonoBehaviour
{
    [SerializeField] private int slotIndex;
    [SerializeField] private TextMeshProUGUI slotLabel;
    [SerializeField] private TextMeshProUGUI sceneLabel;
    [SerializeField] private TextMeshProUGUI playTimeLabel;

    public int SlotIndex => slotIndex;
    public SaveData Data { get; private set; }

    void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        Data = SaveManager.Load(slotIndex);
        UpdateView();
    }

    private void UpdateView()
    {
        if (slotLabel != null)
        {
            slotLabel.text = $"Slot {slotIndex + 1}";
        }

        if (sceneLabel != null)
        {
            sceneLabel.text = Data == null ? "Empty" : Data.sceneName;
        }

        if (playTimeLabel != null)
        {
            playTimeLabel.text = Data == null ? "--:--" : FormatPlayTime(Data.playTime);
        }
    }

    private string FormatPlayTime(float seconds)
    {
        int totalMinutes = Mathf.FloorToInt(seconds / 60f);
        int hours = totalMinutes / 60;
        int minutes = totalMinutes % 60;
        return $"{hours:00}:{minutes:00}";
    }
}
