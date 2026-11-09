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
        if (!canControl || GameplayInputLock.IsLocked)
        {
            input = Vector2.zero;

            UpdateAnimation(Vector2.zero);

            return;
        }

        input.x = ReadAxis(KeyCode.LeftArrow, KeyCode.RightArrow);
        input.y = ReadAxis(KeyCode.DownArrow, KeyCode.UpArrow);

        if (input.magnitude > 1)
        {
            input.Normalize();
        }

        UpdateAnimation(input);
    }

    private static float ReadAxis(KeyCode negative, KeyCode positive)
    {
        float value = 0f;

        if (Input.GetKey(negative)) value -= 1f;
        if (Input.GetKey(positive)) value += 1f;

        return value;
    }

    void FixedUpdate()
    {
        if (!canControl || GameplayInputLock.IsLocked)
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
