using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TextTrigger : MonoBehaviour
{
    [Header("Text Event Settings")]
    [Tooltip("Canvas prefab with TextPopup script")]
    [SerializeField] private GameObject textPopupPrefab;

    [Tooltip("Text to display")]
    [TextArea(2, 4)]
    [SerializeField] private string popupMessage = "My flashlight and blanket are broken...";

    [Header("Trigger Settings")]
    [Tooltip("Does text show once per play?")]
    [SerializeField] private bool triggerOnlyOnce = true;

    private bool hasTriggered = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (triggerOnlyOnce && hasTriggered) return;

            hasTriggered = true;

            if (textPopupPrefab != null)
            {
                GameObject popupInstance = Instantiate(textPopupPrefab);
                TextPopup popupScript = popupInstance.GetComponent<TextPopup>();

                if (popupScript != null) popupScript.ShowText(popupMessage);
            }
        }
    }
}
