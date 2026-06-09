using UnityEngine;
using System.Collections;

public class PlayerHP : MonoBehaviour
{
    [Header("Health Settings")]
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
        currentHealth = GameManager.Instance.currentHP;
    }

    public void TakeDamage(int damageAmount, Vector3 damageSourcePosition)
    {
        if (isInvulnerable) return;

        currentHealth -= damageAmount;
        GameManager.Instance.UpdateHP(currentHealth);
        GetComponent<Player>().Audio.PlayHurt();

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
        GetComponent<Player>().Audio.PlayDeath();
        GameManager.Instance.RespawnPlayer();

        Debug.Log("Player died. Respawning...");
    }

    public void HealToMax()
    {
        currentHealth = GameManager.Instance.maxHP;
        GameManager.Instance.UpdateHP(currentHealth);
        Debug.Log("Played fully healed on checkpoint.");
    }
}
