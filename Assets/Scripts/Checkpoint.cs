using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class Checkpoint : MonoBehaviour
{
    [Tooltip("Active checkpoint color")]
    [SerializeField] private Color activeColor = Color.green;

    private SpriteRenderer spriteRenderer;
    private bool isActivated = false;
    private bool playerInRange = false;

    private PlayerInput playerInput;
    private InputAction interactAction;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void Update()
    {
        if (playerInRange && !isActivated && interactAction != null)
        {
            if (interactAction.WasPressedThisFrame())
            {
                ActivateCheckpoint();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            playerInput = collision.GetComponent<PlayerInput>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            interactAction = null;
            playerInput = null;
        }
    }

    private void ActivateCheckpoint()
    {
        isActivated = true;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = activeColor;

            string currentScene = SceneManager.GetActiveScene().name;
            GameManager.Instance.SetCheckpoint(transform.position, currentScene);

            Debug.Log($"New checkpoint set in {currentScene}");
        }


    }
}
