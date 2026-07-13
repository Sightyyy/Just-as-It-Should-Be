using UnityEngine;

public class ObjectSortingRelativeToPlayer : MonoBehaviour
{
    [SerializeField] private Transform player;

    [Header("Assign dari Inspector")]
    [SerializeField] private GameObject pivotObject;
    [SerializeField] private GameObject visualObject;

    [SerializeField] private int inFrontOrder = 1;
    [SerializeField] private int behindOrder = 3;

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

    public void SetPlayer(Transform target)
    {
        player = target;
    }

    private void InitializeComponents()
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

    private void InitializePlayer()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
                player = p.transform;
        }
    }

    private void ApplySorting()
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
