using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ThreadNeedlePickup : MonoBehaviour
{
    [Header("Level Design Elements")]
    [Tooltip("Objects/Walls that disappear once picked up")]
    [SerializeField] private GameObject[] platformsToDisable;
    [Tooltip("Objects/Walls that appear once picked up")]
    [SerializeField] private GameObject[] objectsToEnable;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if(GameManager.Instance != null)
            {
                GameManager.Instance.UnlockBlanketBase();
                GameManager.Instance.UnlockBlanketGlider();
            }

            Player player = other.GetComponent<Player>();
            if (player != null && player.PlayerBlanket != null)
            {
                player.PlayerBlanket.enabled = true;
            }

            foreach (GameObject platform in platformsToDisable)
            {
                if (platform != null) platform.SetActive(false);
            }

            foreach (GameObject obj in objectsToEnable)
            {
                if (obj != null) obj.SetActive(true);
            }

            gameObject.SetActive(false);
            
            Debug.Log("Blanket repaired!");
        }
    }
}
