using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float jumpForce = 12f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;

    private Rigidbody2D rb;
    private bool isGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (groundCheck == null)
            groundCheck = transform.Find("GroundCheck");
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius);

        var kb = Keyboard.current;
        float horizontal = 0f;
        if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) horizontal = -1f;
        if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) horizontal = 1f;

        rb.linearVelocity = new Vector2(horizontal * moveSpeed, rb.linearVelocity.y);

        if ((kb.spaceKey.wasPressedThisFrame || kb.wKey.wasPressedThisFrame) && isGrounded)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        if (horizontal != 0)
            transform.localScale = new Vector3(Mathf.Sign(horizontal), 1, 1);
    }
}
