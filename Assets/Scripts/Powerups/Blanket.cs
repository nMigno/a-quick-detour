using UnityEngine;
using UnityEngine.InputSystem;

public class Blanket : MonoBehaviour
{
    [Header("Blanket Settings")]
    public Sprite blanketSprite;

    private Sprite originalSprite;
    private Player playerNode;
    private PlayerInput playerInput;
    private InputAction blanketAction;
    private SpriteRenderer playerSpriteRenderer;
    private Animator anim;

    private void Awake()
    {
        playerNode = GetComponentInParent<Player>();
        playerInput = GetComponentInParent<PlayerInput>();
        blanketAction = playerInput.actions.FindAction("Blanket");

        if (playerNode != null)
        {
            anim = playerNode.GetComponent<Animator>();
        }
    }

    private void OnEnable()
    {
        if (blanketAction != null)
        {
            blanketAction.started += OnBlanketPressed;
            blanketAction.canceled += OnBlanketReleased;
        }
    }

    private void OnDisable()
    {
        if (blanketAction != null)
        {
            blanketAction.started -= OnBlanketPressed;
            blanketAction.canceled -= OnBlanketReleased;
        }
    }

    private void Start()
    {
        if (playerSpriteRenderer != null)
        {
            originalSprite = playerSpriteRenderer.sprite;
        }
    }

    private void OnBlanketPressed(InputAction.CallbackContext context)
    {
        ActivateBlanket(true);
    }

    private void OnBlanketReleased(InputAction.CallbackContext context)
    {
        ActivateBlanket(false);
    }

    private void ActivateBlanket(bool isActive)
    {
        if (playerNode == null) return;

        playerNode.Health.isProtectedByBlanket = isActive;
        playerNode.Controller.canMove = !isActive;

        if (playerNode.PlayerFlashlight != null && playerNode.PlayerFlashlight.feedbackRenderer != null)
        {
            playerNode.PlayerFlashlight.feedbackRenderer.enabled = !isActive;
        }

        if (playerNode.Audio != null)
        {
            if (isActive) playerNode.Audio.PlayBlanketOn();
            else playerNode.Audio.PlayBlanketOff();
        }

        if (anim != null) anim.SetBool("MantaOn", isActive);
    }
}
