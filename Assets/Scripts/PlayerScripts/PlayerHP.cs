using UnityEngine;
using System.Collections;

public class PlayerHP : MonoBehaviour
{
    [Header("Health Settings")]
    private int currentHealth;

    [Header("Invulnerability (i-frames)")]
    [SerializeField] private float invulnerabilityDuration = 1.5f;
    private bool isInvulnerable = false;

    [HideInInspector] public bool isProtectedByBlanket = false;

    private Vector3 currentRespawnPosition;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        spriteRenderer.enabled = true;
        currentHealth = GameManager.Instance.currentHP;
    }

    public bool TakeDamage(int damageAmount, Vector3 damageSourcePosition, bool bypassInvulnerability = false)
    {
        if ((isInvulnerable && !bypassInvulnerability) || isProtectedByBlanket) return false;

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
            Animator anim = GetComponent<Animator>();
            if (anim != null) anim.SetTrigger("Damaged");

            GetComponent<PlayerController>().ApplyKnockback(damageSourcePosition);
            StartCoroutine(InvulnerabilityRoutine());
        }

        return true;
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
        Animator anim = GetComponent<Animator>();
        if (anim != null) anim.SetTrigger("Death");

        GetComponent<PlayerController>().enabled = false;
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;

        Debug.Log("Player died. Respawning...");
        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        yield return null;

        float animDuration = 1f;
        Animator anim = GetComponent<Animator>();

        if (anim != null)
        {
            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Death"))
            {
                animDuration = stateInfo.length;
            }
        }

        float totalWaitTime = animDuration + 0.5f;

        // Delete this visual patch block when anim is implemented
        // Start of visual patch
        float elapsedTime = 0f;
        Color originalColor = spriteRenderer.color;
        Quaternion originalRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0, 0, 90f);

        while (elapsedTime < totalWaitTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / totalWaitTime;

            transform.rotation = Quaternion.Lerp(originalRotation, targetRotation, t);
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(1f, 0f, t));

            yield return null;
        }

        spriteRenderer.enabled = false;
        // End of visual patch

        // yield return new WaitForSeconds(animDuration + 0.5f);
        GameManager.Instance.RespawnPlayer();
    }

    public void HealToMax()
    {
        currentHealth = GameManager.Instance.maxHP;
        GameManager.Instance.UpdateHP(currentHealth);
        Debug.Log("Played fully healed on checkpoint.");
    }
}
