using UnityEngine;

public class AnalogClockHand : MonoBehaviour
{
    [SerializeField] private RectTransform hand;
    [SerializeField] private float realMinutesPerGameDay = 15f;
    [SerializeField] private float startHour = 0f;
    [SerializeField] private float handRotationsPerGameDay = 2f;
    [SerializeField] private bool clockwise = true;
    [SerializeField] private bool useInitialRotationAsMidnight = true;
    [SerializeField] private float midnightRotationOffset;
    [SerializeField] private bool enableDebugHotkeys = true;
    [SerializeField] private int currentHour;
    [SerializeField] private int currentMinute;
    [SerializeField] private string currentTimeText;

    private float elapsedSeconds;
    private Quaternion midnightRotation;

    void Awake()
    {
        if (hand == null)
        {
            hand = GetComponent<RectTransform>();
        }

        midnightRotation = hand != null && useInitialRotationAsMidnight
            ? hand.localRotation
            : Quaternion.Euler(0f, 0f, midnightRotationOffset);

        elapsedSeconds = Mathf.Repeat(startHour, 24f) / 24f * GetGameDaySeconds();
        UpdateHandRotation();
    }

    void Update()
    {
        HandleDebugHotkeys();

        elapsedSeconds = Mathf.Repeat(elapsedSeconds + Time.unscaledDeltaTime, GetGameDaySeconds());
        UpdateHandRotation();
    }

    public void AddGameHours(float hours)
    {
        elapsedSeconds = Mathf.Repeat(elapsedSeconds + hours / 24f * GetGameDaySeconds(), GetGameDaySeconds());
        UpdateHandRotation();
    }

    private void HandleDebugHotkeys()
    {
        if (!enableDebugHotkeys) return;

        bool ctrlPressed = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        bool shiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        if (ctrlPressed && shiftPressed && Input.GetKeyDown(KeyCode.F10))
        {
            AddGameHours(1f);
        }
    }

    private void UpdateHandRotation()
    {
        if (hand == null) return;

        float dayProgress = elapsedSeconds / GetGameDaySeconds();
        float direction = clockwise ? -1f : 1f;
        Quaternion timeRotation = Quaternion.Euler(0f, 0f, dayProgress * 360f * handRotationsPerGameDay * direction);
        hand.localRotation = midnightRotation * timeRotation;
        UpdateInspectorTime(dayProgress);
    }

    private void UpdateInspectorTime(float dayProgress)
    {
        float totalGameMinutes = dayProgress * 24f * 60f;
        currentHour = Mathf.FloorToInt(totalGameMinutes / 60f) % 24;
        currentMinute = Mathf.FloorToInt(totalGameMinutes % 60f);
        currentTimeText = $"{currentHour:00}:{currentMinute:00}";
    }

    private float GetGameDaySeconds()
    {
        return Mathf.Max(1f, realMinutesPerGameDay * 60f);
    }
}
