using UnityEngine;
using UnityEngine.InputSystem;

public class LightButton : MonoBehaviour
{
    [Header("Button visuals")]
    public SpriteRenderer buttonRenderer;
    public Color offColor = Color.red;
    public Color onColor = Color.green;

    public static bool roomLightOn = false;

    private bool playerInRange = false;

    private void Start()
    {
        roomLightOn = false;
        UpdateButtonVisuals();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (playerInRange && context.started)
        {
            ToggleLight();
        }
    }

    private void ToggleLight()
    {
        roomLightOn = !roomLightOn;
        UpdateButtonVisuals();
        Debug.Log("Room light: " + (roomLightOn ? "ON" : "OFF"));
    }

    private void UpdateButtonVisuals()
    {
        if (buttonRenderer != null)
        {
            buttonRenderer.color = roomLightOn ? onColor : offColor;
        }
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
