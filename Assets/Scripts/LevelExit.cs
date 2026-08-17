using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BoxCollider2D))]
public class LevelExit : MonoBehaviour
{
    [Header("Transition Settings")]
    [Tooltip("Exact name of scene to load")]
    [SerializeField] private string sceneToLoad;

    [Tooltip("Spawn ID of new scene")]
    [SerializeField] private string destinationDoorID;

    private bool isTransitioning = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isTransitioning)
        {
            if (string.IsNullOrEmpty(sceneToLoad))
            {
                Debug.LogWarning("LevelExit: sceneToLoad not assigned!");
                return;
            }
        }

        isTransitioning = true;

        PlayerInput playerInput = other.GetComponent<PlayerInput>();
        if (playerInput != null) playerInput.DeactivateInput();

        LevelTransition transition = FindAnyObjectByType<LevelTransition>();
        if (transition != null)
        {
            transition.TriggerDoorFade(ExecuteTransition);
        }
        else
        {
            ExecuteTransition();
        }
    }

    private void ExecuteTransition()
    {
        Debug.Log($"Going to {sceneToLoad}, destination: {destinationDoorID}");
        GameManager.Instance.UseDoorTransition(sceneToLoad, destinationDoorID);
    }
}
