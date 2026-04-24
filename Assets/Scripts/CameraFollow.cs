using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Follow")]
    public float smoothTime = 0.2f;
    public Vector2 deadZone = new Vector2(0.5f, 0.3f);

    [Header("Lookahead")]
    public float lookaheadDistance = 2f;
    public float lookaheadSmoothTime = 0.4f;

    [Header("Bounds")]
    public bool hasBounds;
    public Vector2 boundsMin;
    public Vector2 boundsMax;

    private Camera cam;
    private Rigidbody2D targetRb;
    private Vector3 followVelocity;
    private float currentLookahead;
    private float lookaheadVelocity;

    void Awake()
    {
        cam = GetComponent<Camera>();

        if (target == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                target = player.transform;
                targetRb = player.GetComponent<Rigidbody2D>();
            }
        }
        else
        {
            targetRb = target.GetComponent<Rigidbody2D>();
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desired = target.position;
        desired.z = transform.position.z;

        // Lookahead horizontal según velocidad del jugador
        float velX = targetRb != null ? targetRb.linearVelocity.x : 0f;
        float targetLookahead = Mathf.Abs(velX) > 0.1f
            ? Mathf.Sign(velX) * lookaheadDistance
            : 0f;
        currentLookahead = Mathf.SmoothDamp(currentLookahead, targetLookahead, ref lookaheadVelocity, lookaheadSmoothTime);
        desired.x += currentLookahead;

        // Dead zone: la cámara solo se mueve si el jugador salió de la zona central
        Vector3 current = transform.position;
        Vector3 moveTarget = current;

        float dx = desired.x - current.x;
        float dy = desired.y - current.y;

        if (Mathf.Abs(dx) > deadZone.x)
            moveTarget.x = desired.x - Mathf.Sign(dx) * deadZone.x;
        if (Mathf.Abs(dy) > deadZone.y)
            moveTarget.y = desired.y - Mathf.Sign(dy) * deadZone.y;

        Vector3 newPos = Vector3.SmoothDamp(current, moveTarget, ref followVelocity, smoothTime);
        newPos.z = transform.position.z;

        // Clamp a los bounds del cuarto actual
        if (hasBounds)
        {
            float halfH = cam.orthographicSize;
            float halfW = halfH * cam.aspect;

            float minX = boundsMin.x + halfW;
            float maxX = boundsMax.x - halfW;
            float minY = boundsMin.y + halfH;
            float maxY = boundsMax.y - halfH;

            // Si el cuarto es más chico que la cámara, centrar en el cuarto
            newPos.x = minX <= maxX
                ? Mathf.Clamp(newPos.x, minX, maxX)
                : (boundsMin.x + boundsMax.x) * 0.5f;

            newPos.y = minY <= maxY
                ? Mathf.Clamp(newPos.y, minY, maxY)
                : (boundsMin.y + boundsMax.y) * 0.5f;
        }

        transform.position = newPos;
    }

    public void SetBounds(Vector2 min, Vector2 max)
    {
        boundsMin = min;
        boundsMax = max;
        hasBounds = true;
    }

    public void ClearBounds() => hasBounds = false;

    void OnDrawGizmos()
    {
        if (!hasBounds) return;
        Gizmos.color = Color.cyan;
        Vector3 center = new Vector3(
            (boundsMin.x + boundsMax.x) * 0.5f,
            (boundsMin.y + boundsMax.y) * 0.5f, 0);
        Vector3 size = new Vector3(
            boundsMax.x - boundsMin.x,
            boundsMax.y - boundsMin.y, 0);
        Gizmos.DrawWireCube(center, size);
    }
}
