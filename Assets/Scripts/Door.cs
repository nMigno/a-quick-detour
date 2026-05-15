using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;

public class Door : MonoBehaviour
{
    [Header("Door Settings")]
    [Tooltip("Set true if door needs a key to be opened")]
    public string myDoorID;
    
    [Tooltip("Scene destination when door is opened")]
    public string sceneToLoad;

    [Tooltip("Destination Door ID")]
    public string destinationDoorID;

    public bool requiresKey = false;

    private bool playerInRange = false;
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
            if (GameManager.Instance.hasKey)
            {
                isLocked = false;
                Debug.Log("Door unlocked with key!");
                Transition();
            }
            else
            {
                Debug.Log("Needs a key to be opened");
            }
        }
        else
        {
            Transition();
        }
    }

    private void Transition()
    {
        Debug.Log($"Travelling to {sceneToLoad}, destination: {destinationDoorID}");
        GameManager.Instance.UseDoorTransition(sceneToLoad, destinationDoorID);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
