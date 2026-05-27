using UnityEngine;

public class Battery : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        player.PlayerFlashlight.RechargeBattery();
        Destroy(gameObject);
    }
}
