using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    private Door currentDoor;
    private LightButton currentLightButton;

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started && currentDoor != null)
        {
            currentDoor.OnInteract(context);
        }

        if (currentLightButton != null)
        {
            currentLightButton.OnInteract(context);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Door"))
        {
            currentDoor = collision.GetComponent<Door>();
            currentLightButton = collision.GetComponent<LightButton>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Door"))
        {
            currentDoor = null;
            currentLightButton = null;
        }
    }
}
