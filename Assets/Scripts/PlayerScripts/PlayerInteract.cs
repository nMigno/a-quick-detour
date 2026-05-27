using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    private Door currentDoor;
    private Checkpoint currentCheckpoint;

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (currentDoor != null)
            {
                currentDoor.OnInteract(context);
            }

            if (currentCheckpoint != null)
            {
                Player player = GetComponent<Player>();
                currentCheckpoint.Interact(player);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Door"))
        {
            currentDoor = collision.GetComponent<Door>();
        }
        else if (collision.CompareTag("Checkpoint"))
        {
            currentCheckpoint = collision.GetComponent<Checkpoint>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Door"))
        {
            currentDoor = null;
        }
        else if (collision.CompareTag("Checkpoint"))
        {
            currentCheckpoint = null;
        }
    }
}
