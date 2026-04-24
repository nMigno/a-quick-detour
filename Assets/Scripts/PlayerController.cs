using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float jumpForce = 16f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    [Header("Double Jump")]
    public int maxJumps = 2;
    public float doubleJumpCooldown = 0.6f;

    [Header("Ground Check")]
    public float groundCheckWidth = 0.8f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private int jumpsLeft;
    private float jumpCooldownTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapBox(
            new Vector2(transform.position.x, transform.position.y - 0.55f),
            new Vector2(groundCheckWidth, 0.05f),
            0f
        );

        if (isGrounded && rb.linearVelocity.y <= 0.01f)
        {
            jumpsLeft = maxJumps;
            jumpCooldownTimer = 0f;
        }

        if (jumpCooldownTimer > 0f)
            jumpCooldownTimer -= Time.deltaTime;

        var kb = Keyboard.current;
        float horizontal = 0f;
        if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) horizontal = -1f;
        if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) horizontal = 1f;

        rb.linearVelocity = new Vector2(horizontal * moveSpeed, rb.linearVelocity.y);

        bool jumpPressed = kb.spaceKey.wasPressedThisFrame || kb.wKey.wasPressedThisFrame;
        if (jumpPressed && jumpsLeft > 0 && jumpCooldownTimer <= 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpsLeft--;

            // Después del segundo salto, bloquear hasta aterrizar
            if (jumpsLeft == 0)
                jumpCooldownTimer = doubleJumpCooldown;
        }

        if (rb.linearVelocity.y < 0)
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        else if (rb.linearVelocity.y > 0 && !kb.spaceKey.isPressed && !kb.wKey.isPressed)
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;

        if (horizontal != 0)
            transform.localScale = new Vector3(Mathf.Sign(horizontal), 1, 1);
    }
}
