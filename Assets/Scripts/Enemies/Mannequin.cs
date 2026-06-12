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
    private Player player;

    private string enemyID;
    private bool isStunned = false;
    private bool isIlluminated = false;
    private float notLookedTimer = 0f;

    private void Awake()
    {
        enemyID = SceneManager.GetActiveScene().name + "_Mannequin_" + transform.position.ToString();

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        if (normalSprite != null) spriteRenderer.sprite = normalSprite;
    }

    private void Start()
    {
        player = FindAnyObjectByType<Player>();

        if (GameManager.Instance.deadEnemies.Contains(enemyID))
        {
            ApplyStun();

            if (illuminatedSprite != null) spriteRenderer.sprite = illuminatedSprite;
        }
    }

    private void Update()
    {
       if (isStunned || player == null) return;

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
                ChasePlayer();
            }
            else
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
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

    public void ApplyStun()
    {
        if (!isStunned)
        {
            isStunned = true;
            rb.simulated = false;

            if (!string.IsNullOrEmpty(enemyID))
            {
                GameManager.Instance.RegisterDeadEnemy(enemyID);
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
        if (!isStunned && other.CompareTag("Player"))
        {
            PlayerHP playerHP = other.GetComponent<PlayerHP>();

            if (playerHP == null)
            {
                Debug.LogWarning("Warning! Player detected but PlayerHP script is lost");
                return;
            }

            // Aplicar daño. Esto internamente llama a ApplyKnockback() en PlayerController
            playerHP.TakeDamage(damageAmount, transform.position);

            if (audioSource != null && damageClip != null)
            {
                audioSource.pitch = Random.Range(0.9f, 1.1f);
                audioSource.PlayOneShot(damageClip);
            }
        }
    }
}
