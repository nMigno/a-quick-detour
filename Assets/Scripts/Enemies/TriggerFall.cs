using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TriggerFall : MonoBehaviour
{
    [SerializeField] private float fallGravity = 0.1f;
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private bool isFalling = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();

        rb.gravityScale = 0f; // Starts floating

        if (boxCollider != null) boxCollider.isTrigger = true;        
    }

    public void StartFall()
    {
        isFalling = true;
        rb.gravityScale = fallGravity;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(isFalling && other.CompareTag("Ground"))
        {
            if (boxCollider != null) boxCollider.isTrigger = false;
        }
    }
}
