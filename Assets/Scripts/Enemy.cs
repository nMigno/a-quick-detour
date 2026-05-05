using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class Enemy : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private float moveSpeed = 6f;
    [Tooltip("Left patrol limit")]
    [SerializeField] private Transform leftWayPoint;
    [Tooltip("Right patrol limit")]
    [SerializeField] private Transform rightWayPoint;
  
    [Header("Combat Settings")]
    [SerializeField] private int damageAmount = 1;

    [Header("Blockout Color (Test Feedback)")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color illuminatedColor = Color.yellow;
    [SerializeField] private Color stunnedColor = Color.red;

    [Header("Sprites (Artist's Assets)")]
    public Sprite normalSprite;
    public Sprite illuminatedSprite;
    
    private SpriteRenderer spriteRenderer;
    private bool isIlluminated = false;

    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private bool isStunned = false;
    private float stunTimer = 0f;

    private Vector3 leftPos;
    private Vector3 rightPos;

    private void Awake()
    {        
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.color = normalColor;
        if (normalSprite != null)
        {
            spriteRenderer.sprite = normalSprite;
        }    

        moveDirection = Vector2.right;
        leftPos = leftWayPoint.position;
        rightPos = rightWayPoint.position;
    }

    private void Update()
    {
        if (LightButton.roomLightOn)
        {
            rb.linearVelocity = Vector2.zero;

            spriteRenderer.color = illuminatedColor;
            if (illuminatedSprite != null)
            {
                spriteRenderer.sprite = illuminatedSprite;
                return;
            }
        }


        else if (isStunned)
        {
            HandleStunTimer();
            if(stunTimer > 0) return;
        }
        if (isStunned && LightButton.roomLightOn)
            stunTimer = 0f;

        if (!isStunned && !LightButton.roomLightOn) 
        Patrol();
    }

    private void Patrol()
    {
        UpdateVisualFeedback();

        rb.linearVelocity = new Vector2(moveDirection.x * moveSpeed, rb.linearVelocity.y);

        if (moveDirection == Vector2.right && transform.position.x >= rightPos.x)
        {
            FlipDirection(Vector2.left);
        }
        else if (moveDirection == Vector2.left && transform.position.x <= leftPos.x)
        {
            FlipDirection(Vector2.right);
        }
    }

    private void FlipDirection(Vector2 newDirection)
    {
        moveDirection = newDirection;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    // Stun system (public bc is called from Flashlight.cs)
    public void ApplyStun(float duration)
    {
        if (!isStunned)
        {
            isStunned = true;
            rb.linearVelocity = Vector2.zero; // Stop enemy
        }

        stunTimer = duration;
    }


    private void HandleStunTimer()
    {
        stunTimer -= Time.deltaTime;

        if (stunTimer <= 0)
        {
            isStunned = false;
            UpdateVisualFeedback();
        }

    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!isStunned && !LightButton.roomLightOn && other.CompareTag("Player"))
        {
            Debug.Log("Enemy is touching: " + other.gameObject.name + " | Tag: " + other.tag);

            PlayerHP playerHP = other.GetComponent<PlayerHP>();

            if (playerHP == null)
            {
                Debug.LogWarning("Warning! Player detected but PlayerHP script is lost");
            }
            playerHP.TakeDamage(damageAmount, transform.position);
        }
    }

    public void SetIlluminated(bool state, float duration)
    {
        isIlluminated = state;

        if(isIlluminated)
        {
            ApplyStun(duration);
            UpdateVisualFeedback();
        }
        else
        {
            UpdateVisualFeedback();
        }
    }

    private void UpdateVisualFeedback()
    {
        if (isIlluminated)
        {
            spriteRenderer.color = illuminatedColor;
            if (illuminatedSprite != null)
            {
                spriteRenderer.sprite = illuminatedSprite;
            }
        }
        else if (isStunned)
        {
            spriteRenderer.color = stunnedColor;
            if (illuminatedSprite != null)
            {
                spriteRenderer.sprite = illuminatedSprite;
            }
        }
        else
        {
            spriteRenderer.color = normalColor;
            if (normalSprite != null)
            {
                spriteRenderer.sprite = normalSprite;
            }
        }
    }
}
