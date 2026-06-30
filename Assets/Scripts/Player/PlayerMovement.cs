using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 4f;

    private bool canControl = true;

    private Animator animator;
    private Rigidbody2D rb;
    private Vector2 input;

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

            animator.SetFloat("MoveX", 0);
            animator.SetFloat("MoveY", 0);
            animator.SetBool("IsMoving", false);

            return;
        }

        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        animator.SetFloat("MoveX", input.x);
        animator.SetFloat("MoveY", input.y);
        animator.SetBool("IsMoving", input != Vector2.zero);

        if (input.magnitude > 1)
            input.Normalize();
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
}