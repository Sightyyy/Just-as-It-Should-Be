using UnityEngine;

public class ObjectSortingRelativeToPlayer : MonoBehaviour
{
    public Transform player;

    [Header("Assign dari Inspector")]
    public GameObject pivotObject;     // object dengan tag PivotPoint
    public GameObject visualObject;    // object yang punya SpriteRenderer

    public int inFrontOrder = 1;
    public int behindOrder = 3;

    private Transform pivotTransform;
    private SpriteRenderer[] renderers;

    void Awake()
    {
        InitializeComponents();
    }

    void Start()
    {
        InitializePlayer();
    }

    void LateUpdate()
    {
        ApplySorting();
    }

    // =========================
    // INIT
    // =========================
    void InitializeComponents()
    {
        if (pivotObject != null)
        {
            pivotTransform = pivotObject.transform;
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} pivotObject belum diisi!");
        }

        if (visualObject != null)
        {
            renderers = visualObject.GetComponentsInChildren<SpriteRenderer>();
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} visualObject belum diisi!");
        }
    }

    void InitializePlayer()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
                player = p.transform;
        }
    }

    // =========================
    // CORE
    // =========================
    void ApplySorting()
    {
        if (player == null || pivotTransform == null || renderers == null) return;

        float pivotY = pivotTransform.position.y;
        float playerY = player.position.y;

        int targetOrder = (pivotY > playerY) ? inFrontOrder : behindOrder;

        foreach (var sr in renderers)
        {
            sr.sortingOrder = targetOrder;
        }
    }
}