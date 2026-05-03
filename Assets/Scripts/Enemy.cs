using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class Enemy : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private float moveSpeed = 6f;
      [Tooltip("Left patrol limit")]
    [SerializeField] private Transform leftWayPoint;
    [Tooltip("Right patrol limit")]
    [SerializeField] private Transform rightWayPoint;
  

    [Header("Combat Settings")]
    [Tooltip("Stun duration (seconds)")]
    [SerializeField] private float stunDuration = 3f;
    [Tooltip("Damage when touching player")]
    [SerializeField] private int damageAmount = 1;

    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private bool isStunned = false;
    private float stunTimer = 0f;

    private Vector3 leftPos;
    private Vector2 rightPos;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        moveDirection = Vector2.right;

        leftPos = leftWayPoint.position;
        rightPos = rightWayPoint.position;
    }

    private void Update()
    {
        if (isStunned)
        {
            HandleStunTimer();
            return;
        }

        Patrol();
    }

    private void Patrol()
    {
        rb.linearVelocity = new Vector2(moveDirection.x * moveSpeed, rb.linearVelocity.y);

        if (moveDirection == Vector2.right && transform.position.x >= rightPos.x)
        {
            FlipDirection(Vector2.left);
        }
        else if (moveDirection == Vector2.left && transform.position.x <= leftPos.x)
        {
            FlipDirection(Vector2.right);
        }
    }

    private void FlipDirection(Vector2 newDirection)
    {
        moveDirection = newDirection;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    // Stun system (public bc is called from Flashlight.cs)
    public void ApplyStun()
    {
        if (!isStunned)
        {
            isStunned = true;
            rb.linearVelocity = Vector2.zero; // Stop enemy
        }

        stunTimer = stunDuration;
    }

    private void HandleStunTimer()
    {
        if (stunTimer > 0)
        {
            stunTimer -= Time.deltaTime;
        }
        else
        {
            isStunned = false; // Start moving again
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log("Enemy is touching: " + other.gameObject.name + " | Tag: " + other.tag);

        if (!isStunned && other.CompareTag("Player"))
        {
            PlayerHP playerHP = other.GetComponent<PlayerHP>();

            if (playerHP == null)
            {
                Debug.LogWarning("Warning! Player detected but PlayerHP script is lost");
            }
            playerHP.TakeDamage(damageAmount, transform.position);
        }
    }
}
