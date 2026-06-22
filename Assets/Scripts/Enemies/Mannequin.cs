using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class Mannequin : MonoBehaviour
{
    [Header("Mannequin Settings")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float timeToMove = 2f;

    [Header("Combat Settings")]
    [SerializeField] private int damageAmount = 1;

    [Header("Sprites")]
    public Sprite normalSprite;
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
    private float notLookedTimer = 0f;

    private void Awake()
    {
        enemyID = SceneManager.GetActiveScene().name + "_Mannequin_" + transform.position.ToString();

        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        if (normalSprite != null) spriteRenderer.sprite = normalSprite;
    }

    private void Start()
    {
        player = FindAnyObjectByType<Player>();

        if (GameManager.Instance.IsEnemyDead(enemyID, out Vector3 savedPos))
        {
            transform.position = savedPos;
            ApplyStun();

            if (illuminatedSprite != null) spriteRenderer.sprite = illuminatedSprite;
        }
        else
        {
            SetChasingState(false);
        }
    }

    private void Update()
    {
       if (isStunned || player == null) return;

       bool shouldChase = false;

       if (IsPlayerLooking())
        {
            notLookedTimer = 0f;
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
        else
        {
            notLookedTimer += Time.deltaTime;

            if (notLookedTimer >= timeToMove)
            {
                shouldChase = true;
            }
        }

        if (shouldChase != isChasing) SetChasingState(shouldChase);

        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    private void SetChasingState(bool state)
    {
        isChasing = state;

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

    private void ChasePlayer()
    {        
        float direction = Mathf.Sign(player.transform.position.x - transform.position.x);

        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);

        Vector3 scale = transform.localScale;
        scale.x = (direction < 0) ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    public void ApplyStun(bool registerInGameManager = true)
    {
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

        if (isIlluminated)
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

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!isStunned && isChasing && other.CompareTag("Player"))
        {
            Player playerNode = other.GetComponent<Player>();

            if (playerNode != null)
            {
                if(playerNode.Health.TakeDamage(damageAmount, transform.position))
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
}
