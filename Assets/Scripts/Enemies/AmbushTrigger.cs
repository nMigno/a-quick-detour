//using UnityEditor.ShaderGraph;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AmbushTrigger : MonoBehaviour
{
    [Header("Ambush Settings")]
    [Tooltip("Objects who will activate and start chasing")]
    [SerializeField] private GameObject[] enemiesToActivate;

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
            
            foreach (GameObject enemy in enemiesToActivate)
            {
                enemy.SetActive(true);
            }

            if (triggerOnce) gameObject.SetActive(false);
        }
    }

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color (1f, 0.2f, 0.2f, 0.35f);
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        if (box != null) Gizmos.DrawCube(transform.position + (Vector3)box.offset, box.size);
    }
    #endif
}
