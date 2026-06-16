using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class ChairEnemy : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private float moveSpeed = 6f;
    [Tooltip("Left patrol limit")]
    [SerializeField] private Transform leftWayPoint;
    [Tooltip("Right patrol limit")]
    [SerializeField] private Transform rightWayPoint;

    [Header("Chase Settings (aggro)")]
    [SerializeField] private float chaseSpeed = 7f;
    [Tooltip("Seconds chasing after turning flashlight off or hiding")]
    [SerializeField] private float aggroCooldown = 3f;
    [SerializeField] private float horizontalLoseAggro = 15f;
    [SerializeField] private float verticalLoseAggro = 3f;

  
    [Header("Damage Settings")]
    [SerializeField] private int damageAmount = 1;

    [Header("Sprites (Artist's Assets)")]
    public Sprite normalSprite;
    public Sprite illuminatedSprite;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip damageClip;
    
    private SpriteRenderer spriteRenderer;
    private bool isIlluminated = false;
    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private bool isStunned = false;
    private Vector3 leftPos;
    private Vector3 rightPos;
    private string enemyID;
    private Player playerNode;
    private bool isChasing = false;
    private float currentAggroTimer = 0f;

    private void Awake()
    {        
        enemyID = SceneManager.GetActiveScene().name + "_Enemy_" + transform.position.ToString();

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        if (normalSprite != null)
        {
            spriteRenderer.sprite = normalSprite;
        }    

        moveDirection = Vector2.right;
        leftPos = leftWayPoint.position;
        rightPos = rightWayPoint.position;
    }

    private void Start()
    {
        playerNode = FindAnyObjectByType<Player>();

        if (GameManager.Instance.deadEnemies.Contains(enemyID))
        {
            ApplyStun();            
        }

        UpdateVisualFeedback();
    }

    private void Update()
    {
        if (isStunned) return;

        CheckAggroState();

        if (isChasing) ChasePlayer();
        else Patrol();
    }

    private void CheckAggroState()
    {
        bool shouldAggro = false;
        bool isFlashlightOn = playerNode.PlayerFlashlight.currentState == Flashlight.FlashlightState.On;
        bool isProtected = playerNode.Health.isProtectedByBlanket;

        float horizontalDist = Mathf.Abs(transform.position.x - playerNode.transform.position.x);
        float verticalDist = Mathf.Abs(transform.position.y - playerNode.transform.position.y);

        bool inRange = horizontalDist <= horizontalLoseAggro && verticalDist <= verticalLoseAggro;

        if (isChasing)
        {
            if (inRange && !isProtected) shouldAggro = true;
        }
        else
        {
            if (spriteRenderer.isVisible && isFlashlightOn && inRange && !isProtected)
            {
                shouldAggro = true;
            }
        }

        if (shouldAggro)
        {
            isChasing = true;
            currentAggroTimer = aggroCooldown; // Max timer when light's on.
        }
        else if (isChasing)
        {
            currentAggroTimer -= Time.deltaTime; // If no light or hidden, timer starts

            if (currentAggroTimer <= 0f) isChasing = false; // Back to patrol
        }
    }

    private void ChasePlayer()
    {
        float distanceX = playerNode.transform.position.x - transform.position.x;

        if (Mathf.Abs(distanceX) < 0.15f)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        // Calculate player's position
        float directionX = Mathf.Sign(distanceX);
        Vector2 newDirection = directionX > 0 ? Vector2.right : Vector2.left;

        rb.linearVelocity = new Vector2(newDirection.x * chaseSpeed, rb.linearVelocity.y);

        // Flip sprite if player changed side
        if (moveDirection != newDirection) FlipDirection(newDirection);
    }

    private void Patrol()
    {
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
        float rawScaleX = Mathf.Abs(transform.localScale.x);
        transform.localScale = new Vector3(moveDirection.x > 0 ? rawScaleX : -rawScaleX, transform.localScale.y, transform.localScale.z);
        
        UpdateVisualFeedback();
    }

    public void ApplyStun()
    {
        if (!isStunned)
        {
            isStunned = true;
            isChasing = false;
            rb.simulated = false;

            if (!string.IsNullOrEmpty(enemyID))
            {
                GameManager.Instance.RegisterDeadEnemy(enemyID);
            }

            UpdateVisualFeedback();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!isStunned && other.CompareTag("Player"))
        {
            Debug.Log("Enemy is touching: " + other.gameObject.name + " | Tag: " + other.tag);

            PlayerHP playerHP = other.GetComponent<PlayerHP>();

            if (playerHP == null)
            {
                Debug.LogWarning("Warning! Player detected but PlayerHP script is lost");
                return;
            }
            
            if (playerHP.TakeDamage(damageAmount, transform.position))
            {
                if (audioSource != null) 
                {
                    audioSource.pitch = Random.Range(0.9f, 1.1f);
                    audioSource.PlayOneShot(damageClip);
                }
            }
        }
    }

    public void SetIlluminated(bool state)
    {
        isIlluminated = state;

        if(isIlluminated)
        {
            ApplyStun();
            UpdateVisualFeedback();
        }
        else
        {
            UpdateVisualFeedback();
        }
    }

    private void UpdateVisualFeedback()
    {
        if (isIlluminated || isStunned)
        {
            if (illuminatedSprite != null)
            {
                spriteRenderer.sprite = illuminatedSprite;
            }      
        }
        else
        {
            if (normalSprite != null)
            {
                spriteRenderer.sprite = normalSprite;
            }
        }
    }
}
