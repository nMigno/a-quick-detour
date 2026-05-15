using UnityEngine;

public class KeyPickUp : MonoBehaviour
{
    [Header("Level Elements")]
    public GameObject wallToHide;
    public GameObject platformsToShow;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.UnlockKey();

            Debug.Log("Player picke up key!");

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
