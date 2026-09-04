using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(AudioSource))]
public class SkillsOffTrigger : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioClip blanketBreakSound;
    private AudioSource audioSource;

    private bool hasTriggered = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasTriggered && collision.CompareTag("Player"))
        {
            hasTriggered = true;
            ExecuteSequence(collision.gameObject);
        }
    }

    private void ExecuteSequence(GameObject playerObj)
    {
        Player playerNode = playerObj.GetComponent<Player>();
        PlayerInput playerInput = playerObj.GetComponent<PlayerInput>();

        if (blanketBreakSound != null)
        {
            audioSource.PlayOneShot(blanketBreakSound);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.hasBlanketBase = false;
            GameManager.Instance.hasBlanketGlider = false;
            GameManager.Instance.hasFlashlight = false;
        }

        if (playerNode.PlayerBlanket != null) playerNode.PlayerBlanket.enabled = false;
        if (playerNode.PlayerFlashlight != null)
        {
            playerNode.PlayerFlashlight.lightBeamObject.SetActive(false);
            playerNode.PlayerFlashlight.enabled = false;
        }

        if (playerInput != null)
        {
            InputAction flashlightAction = playerInput.actions.FindAction("Flashlight");
            InputAction blanketAction = playerInput.actions.FindAction("Blanket");
            InputAction glideAction = playerInput.actions.FindAction("Glide");

            if (flashlightAction != null) flashlightAction.Disable();
            if (blanketAction != null) blanketAction.Disable();
            if (glideAction != null) glideAction.Disable();
        }
    }
}
