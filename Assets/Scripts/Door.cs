using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class Door : MonoBehaviour
{
    [Header("Door Settings")]
    [Tooltip("Set true if door needs a key to be opened")]
    public string myDoorID;
    
    [Tooltip("Scene destination when door is opened")]
    public string sceneToLoad;

    [Tooltip("Destination Door ID")]
    public string destinationDoorID;

    [Header("Endgame")]
    public bool isFinalDoor = false;

    [Header("Audio")]
    public AudioClip openSound;
    private AudioSource audioSource;

    private bool playerInRange = false;
    private bool isLocked;

    [SerializeField] private Transform playerSpawn;

    private PlayerInput player;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerInput>();
        
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (playerInRange && context.started)
        {
            StartCoroutine(TryInteract());
            
        }
    }

    private IEnumerator TryInteract()
    {
        player.DeactivateInput();

        if (openSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(openSound);
        }
        
        yield return new WaitForSeconds(0.5f);
        if (playerSpawn != null)
        {
            playerSpawn.position = gameObject.transform.position;
        }
        Transition();
    }

    private void Transition()
    {
        if (isFinalDoor)
        {
            EmergencyUI emergencyUI = FindAnyObjectByType<EmergencyUI>();
            if (emergencyUI != null)
            {
                emergencyUI.TriggerLevelEnd();
            }
            else
            {
                Debug.LogError("EmergencyUI canvas missing!");
            }
            return; 
        }

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
