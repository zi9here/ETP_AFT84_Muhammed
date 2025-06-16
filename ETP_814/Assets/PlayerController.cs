using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float dashForce = 20f;
    public float dashDuration = 0.2f;
    public float pushForce = 500f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isDashing;
    private float dashTimeLeft;
    private Vector2 dashDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        CheckGrounded();

        float moveInput = Input.GetAxisRaw("Horizontal");

        if (!isDashing)
        {
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        }

        if (Input.GetButtonDown("Jump") && isGrounded && !isDashing)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        if (Input.GetKeyDown(KeyCode.E) && !isDashing)
        {
            StartDash(moveInput);
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

    void StartDash(float direction)
    {
        isDashing = true;
        dashTimeLeft = dashDuration;
        dashDirection = new Vector2(direction != 0 ? direction : transform.localScale.x, 0).normalized;
    }

    void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
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