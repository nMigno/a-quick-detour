using UnityEngine;
using System.Collections;

public class PlayerHP : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 2;
    private int currentHealth;

    [Header("Invulnerability (i-frames)")]
    [SerializeField] private float invulnerabilityDuration = 1.5f;
    private bool isInvulnerable = false;

    private Vector3 currentRespawnPosition;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
        currentRespawnPosition = transform.position;
    }

    public void TakeDamage(int damageAmount, Vector3 damageSourcePosition)
    {
        if (isInvulnerable) return;

        currentHealth -= damageAmount;
        Debug.Log("Player received damage. HP left: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            GetComponent<PlayerController>().ApplyKnockback(damageSourcePosition);
            StartCoroutine(InvulnerabilityRoutine());
        }
    }

    private IEnumerator InvulnerabilityRoutine()
    {
        isInvulnerable = true;

        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.5f);

        yield return new WaitForSeconds(invulnerabilityDuration);

        spriteRenderer.color = originalColor;
        isInvulnerable = false;
    }

    private void Die()
    {
        Debug.Log("Player died. Respawning...");
        Respawn();
    }

    private void Respawn()
    {
        currentHealth = maxHealth;
        transform.position = currentRespawnPosition;
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        //StartCoroutine(InvulnerabilityRoutine()); only if an enemy is over checkpoint
    }

    public void SetRespawnPoint(Vector3 newRespawnPosition)
    {
        currentRespawnPosition = newRespawnPosition;
    }
}
