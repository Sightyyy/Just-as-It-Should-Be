using UnityEngine;

public class MapController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject mapPanel;
    [SerializeField] private RectTransform mapRect;

    [SerializeField] private Transform player;
    [SerializeField] private RectTransform playerMarker;

    [SerializeField] private Transform questTarget;
    [SerializeField] private RectTransform questMarker;

    [Header("World Bounds")]
    [SerializeField] private Vector2 worldMin;
    [SerializeField] private Vector2 worldMax;

    [Header("Settings")]
    [SerializeField] private bool rotatePlayerMarker = true;
    [SerializeField] private bool handleInputToggle = true;

    public bool IsOpen => mapPanel != null && mapPanel.activeSelf;

    void Update()
    {
        HandleToggle();

        if (mapPanel.activeSelf)
        {
            UpdatePlayerMarker();
            UpdateQuestMarker();
        }
    }

    void HandleToggle()
    {
        if (handleInputToggle && Input.GetKeyDown(KeyCode.M))
        {
            ToggleMap();
        }
    }

    public void ToggleMap()
    {
        SetMapOpen(!IsOpen);
    }

    public void SetMapOpen(bool open)
    {
        if (mapPanel == null) return;

        mapPanel.SetActive(open);
    }

    public void SetQuestTarget(Transform target)
    {
        questTarget = target;
    }

    void UpdatePlayerMarker()
    {
        Vector2 mapPos = WorldToMapPosition(player.position);
        playerMarker.anchoredPosition = ClampToMap(mapPos);

        if (rotatePlayerMarker)
        {
            playerMarker.rotation = Quaternion.Euler(0, 0, player.eulerAngles.z);
        }
    }

    void UpdateQuestMarker()
    {
        if (questTarget == null) return;

        Vector2 mapPos = WorldToMapPosition(questTarget.position);
        questMarker.anchoredPosition = ClampToMap(mapPos);
    }

    private Vector2 WorldToMapPosition(Vector2 worldPos)
    {
        float normalizedX = Mathf.InverseLerp(worldMin.x, worldMax.x, worldPos.x);
        float normalizedY = Mathf.InverseLerp(worldMin.y, worldMax.y, worldPos.y);

        float mapWidth = mapRect.rect.width;
        float mapHeight = mapRect.rect.height;

        return new Vector2(
            normalizedX * mapWidth,
            normalizedY * mapHeight
        );
    }

    private Vector2 ClampToMap(Vector2 pos)
    {
        float mapWidth = mapRect.rect.width;
        float mapHeight = mapRect.rect.height;

        pos.x = Mathf.Clamp(pos.x, 0, mapWidth);
        pos.y = Mathf.Clamp(pos.y, 0, mapHeight);

        return pos;
    }
}
