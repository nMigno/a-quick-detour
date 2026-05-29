using UnityEngine;
using UnityEngine.SceneManagement;

public class Battery : MonoBehaviour
{
    public string batteryID;

    private void Awake()
    {
        batteryID = SceneManager.GetActiveScene().name + "_Battery_" + transform.position.ToString();
    }

    private void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.collectedBatteries.Contains(batteryID))
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();

        player.PlayerFlashlight.RechargeBattery();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterCollectedBattery(batteryID);
        }

        gameObject.SetActive(false);
    }
}
