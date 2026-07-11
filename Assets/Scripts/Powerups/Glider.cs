using UnityEngine;
using UnityEngine.InputSystem;

public class Glider : MonoBehaviour
{
    private Player playerNode;
    private PlayerInput playerInput;
    private InputAction glideAction;

    private void Awake()
    {
        playerNode = GetComponentInParent<Player>();
        playerInput = GetComponentInParent<PlayerInput>();

        glideAction = playerInput.actions.FindAction("Glide");
    }

    private void OnEnable()
    {
        glideAction.started += OnGlidePressed;
        glideAction.canceled += OnGlideReleased;
    }

    private void OnDisable()
    {
        glideAction.started -= OnGlidePressed;
        glideAction.canceled -= OnGlideReleased;
    }

    private void OnGlidePressed(InputAction.CallbackContext context)
    {
        if (GameManager.Instance != null && GameManager.Instance.hasBlanketGlider)
        {
            if (playerNode != null && playerNode.Controller != null)
            {
                playerNode.Controller.OnGlide(context);
            }
        }
    }

    private void OnGlideReleased(InputAction.CallbackContext context)
    {
        if (playerNode != null && playerNode.Controller != null)
        {
            playerNode.Controller.OnGlide(context);
        }
    }
}
