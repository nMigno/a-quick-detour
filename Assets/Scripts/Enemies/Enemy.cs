using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

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
    [SerializeField] private int damageAmount = 1;

    [Header("Sprites (Artist's Assets)")]
    public Sprite normalSprite;
    public Sprite illuminatedSprite;
    
    private SpriteRenderer spriteRenderer;
    private bool isIlluminated = false;
    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private bool isStunned = false;
    private Vector3 leftPos;
    private Vector3 rightPos;
    private string enemyID;

    private void Awake()
    {        
        enemyID = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name + "_Enemy_" + transform.position.ToString();

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (normalSprite != null)
        {
            spriteRenderer.sprite = normalSprite;
        }    

        moveDirection = Vector2.right;
        leftPos = leftWayPoint.position;
        rightPos = rightWayPoint.position;
    }

    private void Start()
    {
        if (GameManager.Instance.deadEnemies.Contains(enemyID))
        {
            ApplyStun();

            if (illuminatedSprite != null)
            {
                spriteRenderer.sprite = illuminatedSprite;
            }
        }
    }

    private void Update()
    {
        if (isStunned) return;
        Patrol();
    }

    private void Patrol()
    {
        UpdateVisualFeedback();

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
            rb.simulated = false;

            if (!string.IsNullOrEmpty(enemyID))
            {
                GameManager.Instance.RegisterDeadEnemy(enemyID);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!isStunned && other.CompareTag("Player"))
        {
            Debug.Log("Enemy is touching: " + other.gameObject.name + " | Tag: " + other.tag);

            PlayerHP playerHP = other.GetComponent<PlayerHP>();

            if (playerHP == null)
            {
                Debug.LogWarning("Warning! Player detected but PlayerHP script is lost");
            }
            playerHP.TakeDamage(damageAmount, transform.position);
        }
    }

    public void SetIlluminated(bool state)
    {
        isIlluminated = state;

        if(isIlluminated)
        {
            ApplyStun();
            UpdateVisualFeedback();
        }
        else
        {
            UpdateVisualFeedback();
        }
    }

    private void UpdateVisualFeedback()
    {
        if (isIlluminated || isStunned)
        {
            if (illuminatedSprite != null)
            {
                spriteRenderer.sprite = illuminatedSprite;
            }      
        }
        else
        {
            if (normalSprite != null)
            {
                spriteRenderer.sprite = normalSprite;
            }
        }
    }
}
