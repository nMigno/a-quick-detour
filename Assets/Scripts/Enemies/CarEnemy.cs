using UnityEngine;

public class CarEnemy : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private float moveSpeed = 12f;
    [SerializeField] private Transform leftWayPoint;
    [SerializeField] private Transform rightWayPoint;

    [Header("Combat Settings")]
    [SerializeField] private int instakillDamage = 2; // Overkill XD

    [Header("Audio Settings")]
    [SerializeField] private float detectionRadius = 15f; // Warning sound setting
    [SerializeField] private AudioClip engineWarningClip;
    [SerializeField] private AudioClip crashClip; // Maybe overlap with player damage sfx?

    private AudioSource audioSource;
    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private Transform playerTransform;
    private bool warningPlayed = false;

    private float leftPos;
    private float rightPos;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        moveDirection = Vector2.right;

        leftPos = leftWayPoint.position.x;
        rightPos = rightWayPoint.position.x;
    }


    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;
    }

    private void Update()
    {
        Patrol();
        ProximityAudio();
    }

    private void Patrol()
    {
        rb.linearVelocity = new Vector2(moveDirection.x * moveSpeed, rb.linearVelocity.y);

        if (moveDirection == Vector2.right && transform.position.x >= rightPos)
        {
            FlipDirection(Vector2.left);
        }
        else if (moveDirection == Vector2.left && transform.position.x <= leftPos)
        {
            FlipDirection(Vector2.right);
        }
    }

    private void FlipDirection(Vector2 newDirection)
    {
        moveDirection = newDirection;
        Vector2 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void ProximityAudio()
    {
        if (playerTransform == null || engineWarningClip == null) return;

        float distance = Vector2.Distance(transform.position, playerTransform.position);

        bool isPlayerToTheRight = playerTransform.position.x > transform.position.x;
        bool isMovingTowardsPlayer = (isPlayerToTheRight && moveDirection.x > 0) || (!isPlayerToTheRight && moveDirection.x < 0);

        if (distance <= detectionRadius && isMovingTowardsPlayer && !warningPlayed)
        {
            audioSource.PlayOneShot(engineWarningClip);
            warningPlayed = true;
        }
        else if (distance > detectionRadius + 5f || !isMovingTowardsPlayer)
        {
            warningPlayed = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHP playerHP = other.GetComponent<PlayerHP>();
            if (playerHP != null)
            {
                if (playerHP.TakeDamage(instakillDamage, transform.position))
                {
                    if(crashClip != null && !audioSource.isPlaying)
                    {
                        audioSource.PlayOneShot(crashClip);
                    }
                }
            }
        }
    }

    #if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
    #endif
}
