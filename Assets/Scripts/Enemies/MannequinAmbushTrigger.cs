using UnityEditor.ShaderGraph;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class MannequinAmbushTrigger : MonoBehaviour
{
    [Header("Ambush Settings")]
    [Tooltip("Mannequins who will activate and start chasing")]
    [SerializeField] private Mannequin[] mannequinsToAmbush;

    [Tooltip("If active, trigger will deactivate once used")]
    [SerializeField] private bool triggerOnce = true;

    private bool hasTriggered = false;

    private void Awake()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered && triggerOnce) return;

        if (other.CompareTag("Player"))
        {
            hasTriggered = true;
            TriggerAmbush();

            if (triggerOnce) gameObject.SetActive(false);
        }
    }

    private void TriggerAmbush()
    {
        if (mannequinsToAmbush == null || mannequinsToAmbush.Length == 0) return;

        foreach (Mannequin mannequin in mannequinsToAmbush)
        {
            if (mannequin != null)
            {
                if (!mannequin.gameObject.activeSelf)
                {
                    mannequin.gameObject.SetActive(true);
                }

                mannequin.ForceChase();
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color (1f, 0.2f, 0.2f, 0.35f);
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        if (box != null) Gizmos.DrawCube(transform.position + (Vector3)box.offset, box.size);
    }
}
