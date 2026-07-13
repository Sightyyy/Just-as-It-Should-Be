using UnityEngine;

public class WatchHandRotator : MonoBehaviour
{
    [SerializeField] private RectTransform hand;
    [SerializeField] private float realMinutesPerGameDay = 15f;
    [SerializeField, Range(0f, 24f)] private float startingHour = 0f;
    [SerializeField] private bool clockwise = true;
    [SerializeField] private bool useUnscaledTime = false;

    private const float HoursPerDay = 24f;
    private float elapsedSeconds;

    public float CurrentHour
    {
        get
        {
            float dayDurationSeconds = GetDayDurationSeconds();
            float normalizedDay = Mathf.Repeat(elapsedSeconds / dayDurationSeconds, 1f);
            return Mathf.Repeat(startingHour + normalizedDay * HoursPerDay, HoursPerDay);
        }
    }

    void Awake()
    {
        if (hand == null)
        {
            hand = GetComponent<RectTransform>();
        }
    }

    void Update()
    {
        elapsedSeconds += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        UpdateHandRotation();
    }

    public void SetTime(float hour, float minute = 0f)
    {
        float targetHour = Mathf.Repeat(hour + minute / 60f, HoursPerDay);
        startingHour = targetHour;
        elapsedSeconds = 0f;
        UpdateHandRotation();
    }

    private void UpdateHandRotation()
    {
        if (hand == null) return;

        float normalizedDay = CurrentHour / HoursPerDay;
        float direction = clockwise ? -1f : 1f;
        hand.localRotation = Quaternion.Euler(0f, 0f, normalizedDay * 360f * direction);
    }

    private float GetDayDurationSeconds()
    {
        return Mathf.Max(1f, realMinutesPerGameDay * 60f);
    }
}
