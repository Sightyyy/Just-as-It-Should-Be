using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 8f;
    [SerializeField] private Vector3 offset;
    [Tooltip("Objek berisi Collider2D pembatas (dipakai 4 collider paling ujung).")]
    [SerializeField] private Transform boundsRoot;

    private Camera cam;
    private bool hasBounds;
    private Vector2 boundsMin;
    private Vector2 boundsMax;

    public Transform Target => target;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    void Awake()
    {
        cam = GetComponent<Camera>();
        CacheBounds();
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );

        transform.position = ClampToBounds(smoothedPosition);
    }

    private void CacheBounds()
    {
        hasBounds = false;

        if (boundsRoot == null) return;

        Collider2D left = null;
        Collider2D right = null;
        Collider2D bottom = null;
        Collider2D top = null;

        foreach (Collider2D col in boundsRoot.GetComponentsInChildren<Collider2D>(true))
        {
            if (!col.enabled) continue;

            Bounds b = col.bounds;

            if (left == null || b.center.x < left.bounds.center.x) left = col;
            if (right == null || b.center.x > right.bounds.center.x) right = col;
            if (bottom == null || b.center.y < bottom.bounds.center.y) bottom = col;
            if (top == null || b.center.y > top.bounds.center.y) top = col;
        }

        if (left == null) return;

        boundsMin = new Vector2(left.bounds.max.x, bottom.bounds.max.y);
        boundsMax = new Vector2(right.bounds.min.x, top.bounds.min.y);

        hasBounds = boundsMin.x < boundsMax.x && boundsMin.y < boundsMax.y;
    }

    private Vector3 ClampToBounds(Vector3 position)
    {
        if (!hasBounds || cam == null || !cam.orthographic) return position;

        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;

        float minX = boundsMin.x + halfWidth;
        float maxX = boundsMax.x - halfWidth;
        float minY = boundsMin.y + halfHeight;
        float maxY = boundsMax.y - halfHeight;

        position.x = minX <= maxX
            ? Mathf.Clamp(position.x, minX, maxX)
            : (boundsMin.x + boundsMax.x) * 0.5f;

        position.y = minY <= maxY
            ? Mathf.Clamp(position.y, minY, maxY)
            : (boundsMin.y + boundsMax.y) * 0.5f;

        return position;
    }
}
