using UnityEngine;
using UnityEngine.InputSystem;

public class Door : MonoBehaviour
{
    [Header("Door Settings")]
    [Tooltip("Set true if door needs a key to be opened")]
    public bool requiresKey = false; 
    
    [Tooltip("Drag destination door here")]
    public Transform destination;
       
    private bool playerInRange = false;
    private Transform playerTransform;
    private bool isLocked;

    private void Start()
    {
        isLocked = requiresKey;
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (playerInRange && context.started)
        {
            TryInteract();
        }
    }

    private void TryInteract()
    {
        if (isLocked)
        {
            if (KeyPickUp.hasKey)
            {
                isLocked = false;
                Debug.Log("Door unlocked with key!");
                Teleport();
            }
            else
            {
                Debug.Log("Needs a key to be opened");
                // Future sound trigger for door closed
            }
        }
        else
        {
            Teleport();
        }
    }

    private void Teleport()
    {
        if (destination != null && playerTransform != null)
        {
            playerTransform.position = destination.position;
            Debug.Log("Teleported");
        }
        else
        {
            Debug.LogWarning("Destination missing on Door Inspector");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            playerTransform = other.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            playerTransform = null;
        }
    }
}
