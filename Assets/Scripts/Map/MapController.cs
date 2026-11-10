using UnityEngine;

public class MapController : MonoBehaviour
{
    public enum MapAccessRealm
    {
        Real,
        Inner
    }

    [Header("References")]
    [SerializeField] private GameObject mapPanel;
    [SerializeField] private RectTransform mapRect;
    [SerializeField] private RealmManager realmManager;

    [SerializeField] private Transform player;
    [SerializeField] private RectTransform playerMarker;

    [SerializeField] private Transform questTarget;
    [SerializeField] private RectTransform questMarker;

    [Header("World Bounds")]
    [SerializeField] private Vector2 worldMin;
    [SerializeField] private Vector2 worldMax;

    [Header("Marker Positioning")]
    [SerializeField] private MapPositionMode positionMode = MapPositionMode.WorldToMapRatio;
    [Tooltip("Titik di world space yang dipetakan ke tengah map (mode WorldToMapRatio).")]
    [SerializeField] private Vector2 worldOrigin = Vector2.zero;
    [Tooltip("Berapa unit dunia yang diwakili 1 pixel map (mode WorldToMapRatio).")]
    [SerializeField] private float worldToMapRatio = 1.5f;
    [SerializeField] private bool centerOriginOnMap = true;

    [Header("Settings")]
    [SerializeField] private bool rotatePlayerMarker = true;
    [SerializeField] private bool handleInputToggle = true;
    [SerializeField] private MapAccessRealm allowedRealm = MapAccessRealm.Real;

    public bool IsOpen => mapPanel != null && mapPanel.activeSelf;
    public bool CanOpenMap => IsInAllowedRealm();

    public enum MapPositionMode
    {
        WorldBounds,
        WorldToMapRatio
    }

    void OnValidate()
    {
        worldToMapRatio = Mathf.Max(0.001f, worldToMapRatio);
    }

    void Update()
    {
        HandleToggle();

        if (IsOpen && (GameplayInputLock.IsLocked || !IsInAllowedRealm()))
        {
            SetMapOpen(false);
        }

        if (IsOpen)
        {
            UpdatePlayerMarker();
            UpdateQuestMarker();
        }
    }

    void HandleToggle()
    {
        if (!handleInputToggle || GameplayInputLock.IsLocked || UiFocus.HasMenuSelection || !Input.GetKeyDown(KeyCode.M)) return;

        ToggleMap();
    }

    public void ToggleMap()
    {
        if (GameplayInputLock.IsLocked) return;

        if (!IsOpen && !IsInAllowedRealm())
        {
            return;
        }

        SetMapOpen(!IsOpen);
    }

    public void SetMapOpen(bool open)
    {
        if (mapPanel == null) return;

        if (open && (GameplayInputLock.IsLocked || !IsInAllowedRealm()))
        {
            open = false;
        }

        mapPanel.SetActive(open);
    }

    private bool IsInAllowedRealm()
    {
        // If no RealmManager is assigned, don't block map access.
        if (realmManager == null) return true;

        return allowedRealm == MapAccessRealm.Real
            ? realmManager.CurrentRealm == RealmManager.Realm.Real
            : realmManager.CurrentRealm == RealmManager.Realm.Inner;
    }

    public void SetQuestTarget(Transform target)
    {
        questTarget = target;
    }

    void UpdatePlayerMarker()
    {
        if (player == null || playerMarker == null) return;

        Vector2 mapPos = WorldToMapPosition(player.position);
        playerMarker.anchoredPosition = ClampToMap(mapPos);

        if (rotatePlayerMarker)
        {
            playerMarker.rotation = Quaternion.Euler(0, 0, player.eulerAngles.z);
        }
    }

    void UpdateQuestMarker()
    {
        if (questTarget == null || questMarker == null) return;

        Vector2 mapPos = WorldToMapPosition(questTarget.position);
        questMarker.anchoredPosition = ClampToMap(mapPos);
    }

    private Vector2 WorldToMapPosition(Vector2 worldPos)
    {
        return positionMode == MapPositionMode.WorldToMapRatio
            ? WorldToMapPositionByRatio(worldPos)
            : WorldToMapPositionByBounds(worldPos);
    }

    private Vector2 WorldToMapPositionByBounds(Vector2 worldPos)
    {
        if (mapRect == null) return Vector2.zero;

        float normalizedX = Mathf.InverseLerp(worldMin.x, worldMax.x, worldPos.x);
        float normalizedY = Mathf.InverseLerp(worldMin.y, worldMax.y, worldPos.y);

        Rect rect = mapRect.rect;

        return new Vector2(
            Mathf.Lerp(rect.xMin, rect.xMax, normalizedX),
            Mathf.Lerp(rect.yMin, rect.yMax, normalizedY)
        );
    }

    private Vector2 WorldToMapPositionByRatio(Vector2 worldPos)
    {
        if (mapRect == null) return Vector2.zero;

        Vector2 mapPos = (worldPos - worldOrigin) / worldToMapRatio;

        if (!centerOriginOnMap)
        {
            Rect rect = mapRect.rect;
            mapPos += new Vector2(rect.xMin, rect.yMin);
        }

        return mapPos;
    }

    private Vector2 ClampToMap(Vector2 pos)
    {
        if (mapRect == null) return pos;

        Rect rect = mapRect.rect;

        pos.x = Mathf.Clamp(pos.x, rect.xMin, rect.xMax);
        pos.y = Mathf.Clamp(pos.y, rect.yMin, rect.yMax);

        return pos;
    }
}
