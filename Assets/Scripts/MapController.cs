using UnityEngine;

public class MapController : MonoBehaviour
{
    [Header("References")]
    public GameObject mapPanel;
    public RectTransform mapRect;

    public Transform player;
    public RectTransform playerMarker;

    public Transform questTarget;
    public RectTransform questMarker;

    [Header("World Bounds")]
    public Vector2 worldMin;
    public Vector2 worldMax;

    [Header("Settings")]
    public bool rotatePlayerMarker = true;

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
        if (Input.GetKeyDown(KeyCode.M))
        {
            mapPanel.SetActive(!mapPanel.activeSelf);
        }
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

    Vector2 WorldToMapPosition(Vector2 worldPos)
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

    Vector2 ClampToMap(Vector2 pos)
    {
        float mapWidth = mapRect.rect.width;
        float mapHeight = mapRect.rect.height;

        pos.x = Mathf.Clamp(pos.x, 0, mapWidth);
        pos.y = Mathf.Clamp(pos.y, 0, mapHeight);

        return pos;
    }
}