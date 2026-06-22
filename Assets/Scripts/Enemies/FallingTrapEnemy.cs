using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D), typeof(Rigidbody2D))]
public class FallingTrapEnemy : MonoBehaviour
{
    [Header("Trap ID")]
    public string trapID;

    [Header("Sprites")]
    public Sprite normalSprite;
    public Sprite brokenSprite;

    [Header("Falling Settings")]
    public float detectionDistance = 15f;
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
        trapID = SceneManager.GetActiveScene().name + "_Enemy_" + transform.position.ToString();

        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        audioSource = GetComponent<AudioSource>();

        rb.bodyType = RigidbodyType2D.Kinematic;
        spriteRenderer.sprite = normalSprite;
        col.isTrigger = true;
    }

    private void Start()
    {
        if (GameManager.Instance.IsEnemyDead(trapID, out Vector3 savedPos))
        {
            transform.position = savedPos;
            SetAlreadyBroken();
        }
    }

    private void Update()
    {
        if (isBroken || isTriggered) return;

        Vector2 boxSize = new Vector2(col.bounds.size.x, 0.1f);
        RaycastHit2D hit = Physics2D.BoxCast(col.bounds.center, boxSize, 0f, Vector2.down, detectionDistance, playerLayer);

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
        
        if (other.CompareTag("Player"))
        {
            DealDamage(other);
        }
        else if (other.CompareTag("Ground") || ((1 << other.gameObject.layer) & groundLayer) != 0)
        {
            BreakTrap();
        }
    }

    private void BreakTrap()
    {
        if (isBroken) return; 

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
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterDeadEnemy(trapID, transform.position);
        }
    }

    private void DealDamage(Collider2D playerCol)
    {
        if (hasDamagedPlayer) return;

        Player playerNode = playerCol.GetComponent<Player>();
        if (playerNode != null)
        {
            playerNode.Health.TakeDamage(damageAmount, transform.position);
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
