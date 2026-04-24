using UnityEngine;

// Definí el área de un cuarto. Agregá un BoxCollider2D al mismo GameObject.
// Cuando el jugador entre al trigger, la cámara se limita a este cuarto.
[RequireComponent(typeof(Collider2D))]
public class CameraRoom : MonoBehaviour
{
    [Tooltip("Usa los límites del propio Collider2D como bounds del cuarto (recomendado)")]
    public bool autoComputeBounds = true;

    [Tooltip("Solo se usa si autoComputeBounds está desactivado")]
    public Vector2 manualMin;
    public Vector2 manualMax;

    private CameraFollow cameraFollow;
    private Collider2D col;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        col.isTrigger = true;
        cameraFollow = Camera.main?.GetComponent<CameraFollow>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (cameraFollow == null) return;

        Vector2 min, max;
        if (autoComputeBounds)
        {
            Bounds b = col.bounds;
            min = b.min;
            max = b.max;
        }
        else
        {
            min = manualMin;
            max = manualMax;
        }

        cameraFollow.SetBounds(min, max);
    }

    void OnDrawGizmos()
    {
        Collider2D c = GetComponent<Collider2D>();
        if (c == null) return;

        Bounds b = c.bounds;
        Gizmos.color = new Color(0f, 1f, 1f, 0.15f);
        Gizmos.DrawCube(b.center, b.size);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(b.center, b.size);
    }
}
