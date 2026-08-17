using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ArmEnemy : MonoBehaviour
{
    [Header("Variant Settings")]
    [Tooltip("Arm Color")]
    [SerializeField] private Color normalColor = Color.white;
    [Tooltip("If active, inflicts 99 damage")]
    [SerializeField] private bool isInstakill = false;

    [Header("Damage Settings")]
    [SerializeField] private int damageAmount = 1;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip damageClip;

    private SpriteRenderer spriteRenderer;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        if (spriteRenderer != null) spriteRenderer.color = normalColor;
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        DealDamage(other.gameObject);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        DealDamage(other.gameObject);
    }
}
