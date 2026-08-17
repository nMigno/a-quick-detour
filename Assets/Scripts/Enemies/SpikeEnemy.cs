using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpikeEnemy : MonoBehaviour
{
    [Header("Variant Settings")]
    [Tooltip("Spike Color")]
    [SerializeField] private Color normalColor = Color.white;
    [Tooltip("If active, inflicts 99 damage")]
    [SerializeField] private bool isInstakill = false;

    [Header("Combat Settings")]
    [SerializeField] private int damageAmount = 1;
    [SerializeField] private AudioClip damageClip;
    [SerializeField] private AudioSource audioSource;

    [Header("Fall Settings")]
    [SerializeField] private bool canFall = false;
    [SerializeField] private float fallGravity = 1f;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Collider2D col;
    private bool isFalling = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        if (spriteRenderer != null) spriteRenderer.color = normalColor;

        if (rb != null)
        {
            if (canFall)
            {
                rb.gravityScale = 0f;
            }
            else
            {
                rb.bodyType = RigidbodyType2D.Kinematic;
                rb.linearVelocity = Vector2.zero;
            }
        }      
    }

    public void StartFall()
    {
        if (!canFall || isFalling || rb == null) return;

        isFalling = true;
        rb.gravityScale = fallGravity;
    }

    private void DealDamage(GameObject target)
    {
        if (target.CompareTag("Player"))
        {
            Player playerNode = target.GetComponent<Player>();
            if (playerNode != null)
            {
                int damageToApply = isInstakill ? 99 : damageAmount;

                if (playerNode.Health.TakeDamage(damageToApply, transform.position))
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

    private void OnTriggerStay2D(Collider2D other)
    {
        DealDamage(other.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DealDamage(other.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        DealDamage(collision.gameObject);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        DealDamage(collision.gameObject);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(isFalling && other.CompareTag("Ground"))
        {
            if (col != null) col.isTrigger = false;
        }
    }
}
