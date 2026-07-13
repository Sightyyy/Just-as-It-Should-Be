using UnityEngine;

public class WatchCameraAnchor : MonoBehaviour
{
    [SerializeField] private RectTransform watchRect;
    [SerializeField] private Camera targetCamera;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Vector2 padding = new Vector2(32f, 32f);
    [SerializeField] private bool hideWhenCameraInactive = true;
    [SerializeField] private bool useBottomRightPivot = true;
    [SerializeField] private bool scaleWithCameraViewport = true;
    [SerializeField] private float minimumScale = 0.125f;
    [SerializeField] private float maximumScale = 1f;

    private Vector3 initialScale;

    void Awake()
    {
        if (watchRect == null)
        {
            watchRect = GetComponent<RectTransform>();
        }

        if (canvas == null)
        {
            canvas = GetComponentInParent<Canvas>();
        }

        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        if (watchRect != null && useBottomRightPivot)
        {
            watchRect.pivot = new Vector2(1f, 0f);
        }

        if (watchRect != null)
        {
            initialScale = watchRect.localScale;
        }
    }

    void LateUpdate()
    {
        if (watchRect == null || targetCamera == null || canvas == null) return;

        bool cameraVisible = targetCamera.gameObject.activeInHierarchy && targetCamera.enabled;
        if (hideWhenCameraInactive)
        {
            watchRect.gameObject.SetActive(cameraVisible);
        }

        if (!cameraVisible) return;

        Rect cameraRect = targetCamera.pixelRect;
        Vector2 screenPosition = new Vector2(
            cameraRect.xMax - padding.x,
            cameraRect.yMin + padding.y
        );

        RectTransform canvasRect = canvas.transform as RectTransform;
        Camera canvasCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPosition, canvasCamera, out Vector2 localPosition))
        {
            watchRect.anchoredPosition = localPosition;
        }

        if (scaleWithCameraViewport)
        {
            float viewportScale = Mathf.Min(
                cameraRect.width / Screen.width,
                cameraRect.height / Screen.height
            );

            float clampedScale = Mathf.Clamp(viewportScale, minimumScale, maximumScale);
            watchRect.localScale = initialScale * clampedScale;
        }
    }
}
