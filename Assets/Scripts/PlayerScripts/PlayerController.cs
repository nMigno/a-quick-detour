using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

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

    [Header("Glider Settings")]
    [Tooltip("Maximum fall speed while gliding.")]
    [SerializeField] private float glideFallSpeed = -2f;
    public bool isGliding = false;
    private bool isHoldingBlanketButton = false;

    [Header("Player Gravity Settings")]
    [SerializeField] private float fallGravityMultiplier = 2.5f;
    [SerializeField] private float maxFallSpeed = 15f;
    private float defaultGravityScale;

    [Header("Collision Checks")]
    [Tooltip("Here goes child object GroundCheck")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.8f, 0.05f);
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private Transform spawnPoint;

    private Rigidbody2D rb;
    private PlayerInput playerInput;
    private InputAction jumpAction;
    private Animator anim;

    public bool canMove = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultGravityScale = rb.gravityScale;
        playerInput = GetComponent<PlayerInput>();
        jumpAction = playerInput.actions.FindAction("Jump");
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        playerInput.ActivateInput();
    }
    
    // Input System Callbacks    
    public void OnMove(InputAction.CallbackContext context)
    {
        // 1D Axis uses float, not Vector2
        horizontalInput = context.ReadValue<float>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!canMove) return;

        if (context.started)
        {
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

    public void OnBlanket(InputAction.CallbackContext context)
    {
        if (GameManager.Instance == null) return;

        Player playerNode = GetComponent<Player>();

        if (context.started)
        {
            if (isHoldingBlanketButton) return;
            isHoldingBlanketButton = true;

            if (isGrounded && GameManager.Instance.hasBlanketBase)
            {
                if (playerNode != null && playerNode.PlayerBlanket != null)
                {
                    playerNode.PlayerBlanket.ActivateBlanket(true);
                }
            }

            else if (!isGrounded && GameManager.Instance.hasBlanketGlider)
            {
                isGliding = true;

                if (playerNode != null && playerNode.PlayerFlashlight != null)
                {
                    playerNode.PlayerFlashlight.InterruptFlashlight();
                }
            }
        }

        else if (context.canceled)
        {
            isHoldingBlanketButton = false;
            isGliding = false;

            if (playerNode != null && playerNode.PlayerBlanket != null && GameManager.Instance.hasBlanketBase)
            {
                playerNode.PlayerBlanket.ActivateBlanket(false);
            }
        }
    }

    private void Update()
    {
        maxJumps = GameManager.Instance.hasDoubleJump ? 2 : 1;

        IncreaseGravity();
        if (canMove) HandleSpriteFlip();

        UpdateAnimations();
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

        CheckCollisions();
        HandleGliding();

        if (!canMove)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            LimitFallSpeed();
            return;
        }

        rb.linearVelocity = new Vector2(horizontalInput * speed, rb.linearVelocity.y);
        LimitFallSpeed();
    }

    //Divide grounded check method from coyote's.
    private void CheckCollisions()
    {
        RaycastHit2D hit = Physics2D.BoxCast(groundCheck.position, groundCheckSize, 0f, Vector2.down, 0.05f, groundLayer);

        //bool wasGrounded = isGrounded;
        isGrounded = hit.collider != null && hit.normal.y > 0.1f;

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

        GetComponent<Player>().Audio.PlayJump();
    }

    private void IncreaseGravity()
    {
        if (isGliding && !isGrounded && rb.linearVelocity.y <= 0)
        {
            rb.gravityScale = 0f;
        }
        // Apply extra gravity when falling
        else if (!isGrounded && rb.linearVelocity.y < -0.2f)
        {
            rb.gravityScale = defaultGravityScale * fallGravityMultiplier;
        }
        else
        {
            rb.gravityScale = defaultGravityScale;
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

    public void UnlockDoubleJump()
    {
        maxJumps = 2;
        Debug.Log("Double jump unlocked!");
    }

    private void HandleGliding()
    {
        if (isGrounded && isGliding) isGliding = false;

        if (isGliding && !isGrounded && GameManager.Instance.hasBlanketGlider)
        {
            if (rb.linearVelocity.y <= 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, glideFallSpeed);
            }
        }
    }

    private void UpdateAnimations()
    {
        if (anim == null) return;

        anim.SetFloat("Speed", Mathf.Abs(horizontalInput));
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
        anim.SetBool("IsGrounded", isGrounded);
        anim.SetBool("IsGliding", isGliding);
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

    private void LimitFallSpeed()
    {
        if (rb.linearVelocity.y < -maxFallSpeed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -maxFallSpeed);
        }
    }
}
