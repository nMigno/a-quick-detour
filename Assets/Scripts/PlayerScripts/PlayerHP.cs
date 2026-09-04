using UnityEngine;
using System.Collections;

public class PlayerHP : MonoBehaviour
{
    [Header("Health Settings")]
    private int currentHealth;

    public int CurrentHealth => currentHealth;

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

    public void TriggerVoidDeath()
    {
        currentHealth = 0;
        GameManager.Instance.UpdateHP(currentHealth);
        Die(true);
    }

    private void Die(bool isVoidDeath = false)
    {      
        GetComponent<Player>().Audio.PlayDeath();

        gameObject.tag = "Untagged";

        UnityEngine.InputSystem.PlayerInput playerInput = GetComponent<UnityEngine.InputSystem.PlayerInput>();
        if (playerInput != null) 
        {
            playerInput.DeactivateInput();
            playerInput.enabled = false;
        }

        PlayerController controller = GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.canMove = false;
            controller.enabled = false;
        }

        if (!isVoidDeath)
        {
            GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        }

        Debug.Log("Player died. Respawning...");

        Animator anim = GetComponent<Animator>();
        if (anim != null) anim.SetTrigger("Death");

        
        StartCoroutine(DeathRoutine(isVoidDeath));
    }

    private IEnumerator DeathRoutine(bool isVoidDeath)
    {
        if (isVoidDeath) TriggerFadeAndRespawn();

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

        float totalWaitTime = isVoidDeath ? 0.5f : (animDuration + 0.5f);

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

        // After animation, fade
        if (!isVoidDeath) TriggerFadeAndRespawn();
    }

    private void TriggerFadeAndRespawn()
    {
        LevelTransition transition = FindFirstObjectByType<LevelTransition>();

        if (transition != null)
        {
            transition.TriggerDoorFade(() => GameManager.Instance.RespawnPlayer());
        }
        else
        {
            GameManager.Instance.RespawnPlayer();
        }
    }

    public void HealToMax()
    {
        currentHealth = GameManager.Instance.maxHP;
        GameManager.Instance.UpdateHP(currentHealth);
        Debug.Log("Played fully healed on checkpoint.");
    }
}
