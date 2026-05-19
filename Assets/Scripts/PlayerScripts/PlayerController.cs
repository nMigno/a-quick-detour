using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 8f;
    private float horizontalInput;
    private bool isFacingRight = true;

    [Header("Knockback Settings")]
    [SerializeField] private float knockbackForceX = 10f;
    [SerializeField] private float knockbackForceY = 5f;
    [SerializeField] private float knockbackDuration = 0.2f;    
    private bool isKnockedBack = false;
    private float knockbackTimer = 0f;
  
    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 20f;
    [SerializeField] private float jumpCutMultiplier = 0.5f;
    [SerializeField] private int maxJumps = 1;
    [SerializeField] private float coyoteTime = 0.15f;
    private int jumpsRemaining;
    private bool isGrounded;
    private float coyoteTimeCounter;

    [Header("Player Gravity Settings")]
    [SerializeField] private float fallGravityMultiplier = 2.5f;
    private float defaultGravityScale;

    [Header("Collision Checks")]
    [Tooltip("Here goes child object GroundCheck")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.9f, 0.05f);
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private Transform spawnPoint;

    private Rigidbody2D rb;
    private PlayerInput playerInput;
    private InputAction jumpAction;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultGravityScale = rb.gravityScale;
        playerInput = GetComponent<PlayerInput>();
        jumpAction = playerInput.actions.FindAction("Jump");
    }

    private void Start()
    {
        gameObject.transform.position = spawnPoint.position;
        playerInput.ActivateInput();
        // Set Max Health/Sanity
        // Set UI Manager
        // Initialize Checkpoints
        // Initialize Flashlight battery/system
    }
    
    // Input System Callbacks    
    public void OnMove(InputAction.CallbackContext context)
    {
        // 1D Axis uses float, not Vector2
        horizontalInput = context.ReadValue<float>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            /*if (coyoteTimeCounter > 0)
            {
               Jump();
            }*/
            if (jumpsRemaining > 0)
            {
                Jump();
            }
        }

        if (context.canceled)
        {
            if (rb.linearVelocity.y > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
            }
        }
    }
    private void Update()
    {
        IncreaseGravity();
        HandleSpriteFlip();
    }

    private void FixedUpdate()
    {
        if (isKnockedBack)
        {
            knockbackTimer -= Time.fixedDeltaTime;
            if (knockbackTimer <= 0f)
            {
                isKnockedBack = false;
            }
            return; // we ignore inputs returning
        }

        rb.linearVelocity = new Vector2(horizontalInput * speed, rb.linearVelocity.y);
        CheckCollisions();
    }

    //Seoarar funcion de chequear grounded de funcion coyote
    private void CheckCollisions()
    {
        //bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer);
        bool isHoldingJump = jumpAction.IsPressed();

        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;

            if(rb.linearVelocity.y <= 0.1f && !isHoldingJump)
            {
                jumpsRemaining = maxJumps;
            }    
        }
        else
        {
            // If in the air, counter starts decreasing
            coyoteTimeCounter -= Time.deltaTime;

            // Avoid air jump if counter = 0
            if (coyoteTimeCounter <= 0f && jumpsRemaining == maxJumps)
            {
                jumpsRemaining = 0;
            }
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        coyoteTimeCounter = 0f; // Avoids double jump
        jumpsRemaining--;
    }

    private void IncreaseGravity()
    {
        // Apply extra gravity when falling
        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = defaultGravityScale * fallGravityMultiplier;
        }
        else
        {
            rb.gravityScale = defaultGravityScale;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            //Future enemy interactions
        }

    }
    
    private void HandleSpriteFlip()
    {
        if (horizontalInput > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (horizontalInput < 0 && isFacingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector2 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public void ApplyKnockback(Vector3 damageSourcePosition)
    {
        isKnockedBack = true;
        knockbackTimer = knockbackDuration;

        // Check from which side player got hit
        float horizontalDirection = (transform.position.x < damageSourcePosition.x) ? -1f : 1f;

        // Reset speed for consistent rebound
        rb.linearVelocity = Vector2.zero;

        // Impulse for hit player 
        rb.AddForce(new Vector2(horizontalDirection * knockbackForceX, knockbackForceY), ForceMode2D.Impulse);
    }

    //Editor box rendering
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
        }
    }
}
