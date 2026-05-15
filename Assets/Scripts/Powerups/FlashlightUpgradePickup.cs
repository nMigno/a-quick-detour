using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FlashlightUpgradePickup : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.UnlockFlashlightUpgrade();
            Debug.Log("Player picked up flashlight upgrade!");
            gameObject.SetActive(false);
        }
    }
}
