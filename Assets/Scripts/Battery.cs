using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class Battery : MonoBehaviour
{
    [Header("Pickup Events")]
    public UnityEvent OnPickedUp;

    [Header("Text Event Settings")]
    [Tooltip("Canvas prefab with TextPopup script")]
    [SerializeField] private GameObject textPopupPrefab;

    [Tooltip("Text to display when picked up")]
    [TextArea(2, 4)]
    [SerializeField] private string popupMessage = "Battery acquired!";

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void Start()
    {
        if (GameManager.Instance.hasFlashlight)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RepairFlashlight();
            }

            Player playerNode = collision.GetComponent<Player>();
            if (playerNode != null && playerNode.PlayerFlashlight != null)
            {
                playerNode.PlayerFlashlight.enabled = true;

                if (playerNode.PlayerFlashlight.lightBeamObject != null)
                {
                    playerNode.PlayerFlashlight.lightBeamObject.SetActive(true);
                }
            }
            
            OnPickedUp?.Invoke();

            if (textPopupPrefab != null)
            {
                GameObject popupInstance = Instantiate(textPopupPrefab);
                TextPopup popupScript = popupInstance.GetComponent<TextPopup>();

                if (popupScript != null) popupScript.ShowText(popupMessage);
            }

            gameObject.SetActive(false);
        }
    }
}