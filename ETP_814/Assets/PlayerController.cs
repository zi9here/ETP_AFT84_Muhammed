using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float dashForce = 20f;
    public float dashDuration = 0.2f;
    public float pushForce = 500f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded;
    private bool isDashing;
    private float dashTimeLeft;
    private Vector2 dashDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        CheckGrounded();
        HandleMovement();
        HandleJump();
        HandleDash();
        UpdateAnimations();
    }

    void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    void HandleMovement()
    {
        if (isDashing) return;

        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // Flip character sprite
        if (moveInput != 0)
            transform.localScale = new Vector3(Mathf.Sign(moveInput), 1, 1);
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded && !isDashing)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isDashing)
        {
            float moveDir = Input.GetAxisRaw("Horizontal");
            dashDirection = new Vector2(moveDir != 0 ? moveDir : transform.localScale.x, 0).normalized;
            dashTimeLeft = dashDuration;
            isDashing = true;

            anim.SetTrigger("Dash");
        }

        if (isDashing)
        {
            rb.linearVelocity = dashDirection * dashForce;
            dashTimeLeft -= Time.deltaTime;
            if (dashTimeLeft <= 0)
            {
                isDashing = false;
            }
        }
    }

    void UpdateAnimations()
    {
        anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDashing && collision.gameObject.CompareTag("Pushable"))
        {
            Rigidbody2D otherRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (otherRb != null)
            {
                Vector2 forceDir = (collision.transform.position - transform.position).normalized;
                otherRb.AddForce(forceDir * pushForce);
            }
        }
    }
}