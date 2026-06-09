using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D), typeof(Rigidbody2D))]
public class CupEnemy : MonoBehaviour
{
    [Header("CupID")]
    public string cupID;

    [Header("Sprites")]
    public Sprite normalSprite;
    public Sprite brokenSprite;

    [Header("Falling Settings")]
    public float detectionDistance = 5f;
    public float brokenDetectionDistance = 20f;
    public float warningTime = 0.5f;
    public float fallGravity = 4f;

    [Header("Damage Settings")]
    [SerializeField] private int damageAmount = 1;

    [Header("Layers")]
    public LayerMask playerLayer;
    public LayerMask groundLayer;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip warningClip;
    public AudioClip breakClip;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Collider2D col;

    private bool isTriggered = false;
    private bool isBroken = false;
    private bool isIlluminated = false;
    private bool hasDamagedPlayer = false;

    private void Awake()
    {
        cupID = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name + "_Enemy_" + transform.position.ToString();

        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        rb.bodyType = RigidbodyType2D.Kinematic;
        spriteRenderer.sprite = normalSprite;
        col.isTrigger = true;
    }

    private void Start()
    {
        if (GameManager.Instance.deadEnemies.Contains(cupID))
        {
            SetAlreadyBroken();
        }
    }

    private void Update()
    {
        if (isBroken || isTriggered) return;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, detectionDistance, playerLayer);

        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            StartCoroutine(TriggerFall());
        }
    }

    private IEnumerator TriggerFall()
    {
        isTriggered = true;

        if (audioSource != null && warningClip != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(warningClip);
        }

        Vector3 originalPos = transform.position;
        float elapsed = 0f;
        while (elapsed < warningTime)
        {
            transform.position = originalPos + (Vector3)Random.insideUnitCircle * 0.05f;
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = originalPos;

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = fallGravity;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isBroken) return;
        
        if (other.CompareTag("Ground"))
        {
            BreakCup();
        }
    }

    private void FixedUpdate()
    {
        if (isBroken || !isTriggered) return;

        Collider2D playerCollider = Physics2D.OverlapBox(col.bounds.center, col.bounds.size, 0f, playerLayer);
        
        if (playerCollider != null && playerCollider.CompareTag("Player"))
        {
            DealDamage(playerCollider);
        }
    }

    private void BreakCup()
    {
        if (isBroken) return; 

        if (!hasDamagedPlayer)
        {
            Collider2D playerCollider = Physics2D.OverlapBox(col.bounds.center, col.bounds.size, 0f, playerLayer);
            if (playerCollider != null && playerCollider.CompareTag("Player"))
            {
                DealDamage(playerCollider);
            }
        }

        isBroken = true;

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;  
        col.enabled = false;
        spriteRenderer.sprite = brokenSprite;

        if (audioSource != null && breakClip != null) 
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(breakClip);
        }
        
        GameManager.Instance.deadEnemies.Add(cupID);
    }

    private void DealDamage(Collider2D playerCol)
    {
        if (hasDamagedPlayer) return;

        PlayerHP playerHP = playerCol.GetComponent<PlayerHP>();
        if (playerHP != null)
        {
            playerHP.TakeDamage(damageAmount, transform.position);
            hasDamagedPlayer = true;
        }
    }

    private void SetAlreadyBroken()
    {
        isBroken = true;
        isTriggered = true;
        rb.bodyType = RigidbodyType2D.Static;
        col.enabled = false;
        spriteRenderer.sprite = brokenSprite;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, brokenDetectionDistance, groundLayer);
        if (hit.collider != null)
        {
            transform.position = hit.point + Vector2.up * 0.2f;
        }
    }

    public void SetIlluminated(bool state)
    {
        isIlluminated = state;

        if (!isBroken)
        {
            spriteRenderer.sprite = state ? brokenSprite : normalSprite;
        }
    }
}
