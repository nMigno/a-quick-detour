using UnityEngine;

public class DoubleJumpPowerUp : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        GameManager.Instance.UnlockDoubleJump();
        player.Controller.UnlockDoubleJump();
        Destroy(gameObject);
    }
}
