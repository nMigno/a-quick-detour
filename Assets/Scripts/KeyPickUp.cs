using UnityEngine;

public class KeyPickUp : MonoBehaviour
{
    [Header("Level Elements")]
    [Tooltip("Drag here dissapearing barrier walls")]
    public GameObject wallToHide;
    [Tooltip("Drag here appearing platforms")]
    public GameObject platformsToShow;

    // This has to be in a future StateManager.cs
    [Header("Placeholder inventory")]
    public static bool hasKey = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            hasKey = true;

            if (wallToHide != null)
            {
                wallToHide.SetActive(false);
            }

            if (platformsToShow != null)
            {
                platformsToShow.SetActive(true);
            }

            gameObject.SetActive(false);
        }
    }
}
