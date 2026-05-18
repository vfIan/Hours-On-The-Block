using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController2D : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Visual")]
    public Transform bodyVisual;
    public Animator animator;

    private Rigidbody2D rb;
    private bool isGrounded;
    private Vector2 moveInput;
    private Vector3 bodyOriginalScale;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (bodyVisual != null)
            bodyOriginalScale = bodyVisual.localScale;
    }

    void Update()
    {
        CheckGround();
        FlipCharacter();
        UpdateAnimation();
    }

    void FixedUpdate()
    {
        Move();
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (!value.isPressed) return;

        if (isGrounded && rb != null)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    void Move()
    {
        if (rb == null) return;

        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
    }

    void CheckGround()
    {
        if (groundCheck == null)
        {
            isGrounded = false;
            return;
        }

        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
    }

    void FlipCharacter()
    {
        if (bodyVisual == null) return;

        if (moveInput.x > 0.01f)
        {
            Vector3 scale = bodyOriginalScale;
            scale.x = Mathf.Abs(bodyOriginalScale.x);
            bodyVisual.localScale = scale;
        }
        else if (moveInput.x < -0.01f)
        {
            Vector3 scale = bodyOriginalScale;
            scale.x = -Mathf.Abs(bodyOriginalScale.x);
            bodyVisual.localScale = scale;
        }
    }

    void UpdateAnimation()
    {
        if (animator == null || rb == null) return;

        bool isRunning = Mathf.Abs(moveInput.x) > 0.01f && isGrounded;

        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("yVelocity", rb.linearVelocity.y);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}