using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class Mannequin : MonoBehaviour
{
    [Header("Mannequin Settings")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float timeToMove = 2f;
    [Tooltip("Max detección range X")]
    [SerializeField] private float detectionRangeX = 15f;
    [Tooltip("Max detection range Y")]
    [SerializeField] private float detectionRangeY = 3f;

    [Header("Edge & Ground Detection")]
    [Tooltip("These avoid not falling into void")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.6f;

    [Header("Variant Settings")]
    [SerializeField] private bool isInstakill = false;
    [SerializeField] private Color normalColor = Color.white;

    [Header("Combat Settings")]
    [SerializeField] private int damageAmount = 1;

    [Header("Sprites")]
    public Sprite normalSprite;
    public Sprite chasingSprite;
    public Sprite illuminatedSprite;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip damageClip;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Collider2D col;
    private Player player;

    private string enemyID;
    private bool isStunned = false;
    private bool isIlluminated = false;
    private bool isChasing = false;
    private bool isForcedChase = false;
    private float notLookedTimer = 0f;

    private void Awake()
    {
        enemyID = SceneManager.GetActiveScene().name + "_Mannequin_" + transform.position.ToString();

        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        player = FindAnyObjectByType<Player>();

        UpdateVisualFeedback();
    }

    private void Start()
    { 
        if (GameManager.Instance.IsEnemyDead(enemyID, out Vector3 savedPos))
        {
            transform.position = savedPos;
            ApplyStun();

            if (illuminatedSprite != null) spriteRenderer.sprite = illuminatedSprite;
        }
        else if (!isForcedChase)
        {
            SetChasingState(false);
        }
    }

    private void Update()
    {
       if (isStunned) return;

        if (player == null || IsPlayerDead())
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

       bool shouldChase = false;

       if (isForcedChase)
       {
           shouldChase = true;
       }
        else
        {
            float distanceX = Mathf.Abs(transform.position.x - player.transform.position.x);
            float distanceY = Mathf.Abs(transform.position.y - player.transform.position.y);
            bool isInRange = (distanceX <= detectionRangeX) && (distanceY <= detectionRangeY);

            if (isInRange)
            {
                if (IsPlayerLooking())
                {
                    notLookedTimer = 0f;
                    rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                }
                else
                {
                    notLookedTimer += Time.deltaTime;
                    if (notLookedTimer >= timeToMove) shouldChase = true;
                }
            }
            else
            {
                notLookedTimer = 0f;
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }
        }    
       
        if (shouldChase != isChasing) SetChasingState(shouldChase);        
        if (isChasing) ChasePlayer();
    }

    private void SetChasingState(bool state)
    {
        if (isChasing == state) return;

        isChasing = state;
        UpdateVisualFeedback();

        if (player != null)
        {
            Collider2D playerCollider = player.GetComponent<Collider2D>();

            if (playerCollider != null && col != null)
            {
                Physics2D.IgnoreCollision(col, playerCollider, !isChasing);
            }
        }
    }

    private bool IsPlayerLooking()
    {
        // Check if enemy is to left or right of player
        float directionToEnemy = transform.position.x - player.transform.position.x;
        // Check where is player looking basing on scale
        bool playerFacingRight = player.transform.localScale.x > 0;
        return (directionToEnemy > 0 && playerFacingRight) || (directionToEnemy < 0 && !playerFacingRight);
    }

    private bool IsPlayerDead()
    {
        return player.Health != null && player.Health.CurrentHealth <= 0;
    }

    private bool HasGroundAhead(float direction)
    {
        Bounds bounds = col.bounds;
        Vector2 checkOrigin = new Vector2(bounds.center.x + direction * (bounds.extents.x + 0.1f), bounds.min.y + 0.1f);
        RaycastHit2D hit = Physics2D.Raycast(checkOrigin, Vector2.down, groundCheckDistance, groundLayer);

        return hit.collider != null;
    }

    private void ChasePlayer()
    {        
        float direction = Mathf.Sign(player.transform.position.x - transform.position.x);

        if (!HasGroundAhead(direction))
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);

        Vector3 scale = transform.localScale;
        scale.x = (direction < 0) ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    public void ApplyStun(bool registerInGameManager = true)
    {
        if (isForcedChase) return;

        if (!isStunned)
        {
            isStunned = true;
            isChasing = false;
            rb.simulated = false;

            if (registerInGameManager && !string.IsNullOrEmpty(enemyID))
            {
                GameManager.Instance.RegisterDeadEnemy(enemyID, transform.position);
            }
        }
    }

    public void SetIlluminated(bool state)
    {
        isIlluminated = state;
        UpdateVisualFeedback();
    }

    public void ForceChase()
    {
        isForcedChase = true;
        isStunned = false;
        rb.simulated = true;
        SetChasingState(true);
    }

    private void UpdateVisualFeedback()
    {
        if (isIlluminated || isStunned)
        {
            if (illuminatedSprite != null)
            {
                spriteRenderer.sprite = illuminatedSprite;
                spriteRenderer.color = Color.white;
            }
        }
        else if (isChasing)
        {
            if (chasingSprite != null)
            {
                spriteRenderer.sprite = chasingSprite;
                spriteRenderer.color = normalColor;
            }
        }
        else
        {
            if (normalSprite != null)
            {
                spriteRenderer.sprite = normalSprite;
                spriteRenderer.color = normalColor;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!isStunned && isChasing && other.CompareTag("Player"))
        {
            Player playerNode = other.GetComponent<Player>();

            if (playerNode != null)
            {
                int damageApplied = isInstakill ? 99 : damageAmount;

                if(playerNode.Health.TakeDamage(damageApplied, transform.position))
                {
                    if (audioSource != null && damageClip != null)
                    {
                        audioSource.pitch = Random.Range(0.9f, 1.1f);
                        audioSource.PlayOneShot(damageClip);
                    }
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(detectionRangeX * 2f, detectionRangeY * 2f, 0f));

        if (col != null)
        {
            Gizmos.color = Color.red;
            Bounds bounds = col.bounds;
            Vector2 checkLeft = new Vector2(bounds.center.x - (bounds.extents.x + 0.1f), bounds.min.y + 0.1f);
            Vector2 checkRight = new Vector2(bounds.center.x + (bounds.extents.x + 0.1f), bounds.min.y + 0.1f);

            Gizmos.DrawLine(checkLeft, checkLeft + Vector2.down * groundCheckDistance);
            Gizmos.DrawLine(checkRight, checkRight + Vector2.down * groundCheckDistance);
        }
    }
}
