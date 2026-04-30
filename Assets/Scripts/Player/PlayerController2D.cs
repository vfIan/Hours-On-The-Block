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
    private float moveInput = 0f;
    private Vector3 bodyOriginalScale;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (bodyVisual != null)
            bodyOriginalScale = bodyVisual.localScale;
    }

    void Update()
    {
        ReadInput();
        CheckGround();
        Jump();
        FlipCharacter();
        UpdateAnimation();
    }

    void FixedUpdate()
    {
        Move();
    }

    void ReadInput()
    {
        moveInput = 0f;

        if (Keyboard.current == null) return;

        if (Keyboard.current.aKey.isPressed)
            moveInput = -1f;
        else if (Keyboard.current.dKey.isPressed)
            moveInput = 1f;
    }

    void Move()
    {
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
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

    void Jump()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    void FlipCharacter()
    {
        if (bodyVisual == null) return;

        if (moveInput > 0)
        {
            Vector3 scale = bodyOriginalScale;
            scale.x = Mathf.Abs(bodyOriginalScale.x);
            bodyVisual.localScale = scale;
        }
        else if (moveInput < 0)
        {
            Vector3 scale = bodyOriginalScale;
            scale.x = -Mathf.Abs(bodyOriginalScale.x);
            bodyVisual.localScale = scale;
        }
    }

    void UpdateAnimation()
    {
        if (animator == null) return;

        bool isRunning = moveInput != 0f && isGrounded;

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