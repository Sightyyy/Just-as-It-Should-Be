using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 4f;

    private bool canControl = true;

    private Animator animator;
    private Rigidbody2D rb;
    private Vector2 input;

    public bool CanControl => canControl;
    public Vector2 MoveInput => input;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (!canControl)
        {
            input = Vector2.zero;

            UpdateAnimation(Vector2.zero);

            return;
        }

        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        if (input.magnitude > 1)
        {
            input.Normalize();
        }

        UpdateAnimation(input);
    }

    void FixedUpdate()
    {
        if (!canControl)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.linearVelocity = input * moveSpeed;
    }

    public void SetControl(bool value)
    {
        canControl = value;
    }

    private void UpdateAnimation(Vector2 movement)
    {
        if (animator == null) return;

        animator.SetFloat("MoveX", movement.x);
        animator.SetFloat("MoveY", movement.y);
        animator.SetBool("IsMoving", movement != Vector2.zero);
    }
}
