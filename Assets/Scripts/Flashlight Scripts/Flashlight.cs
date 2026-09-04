using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Flashlight : MonoBehaviour
{
    // Flashlight states
    public enum FlashlightState { Ready, On, Jammed }
    public FlashlightState currentState = FlashlightState.Ready;

    [Header("Time (seconds)")]
    public float activeDuration = 1f;
    public float cooldownDuration = 3f;

    [Header("References")]
    public GameObject lightBeamObject; // Stun trigger
    public SpriteRenderer feedbackRenderer; // Flashlight visual

    [Header("Visual feedback")]
    public Color readyColor = Color.green;
    public Color onColor = Color.white;
    public Color jammedColor = Color.red;

    private Player playerNode;
    private PlayerInput playerInput;
    private InputAction flashlightAction;

    private Coroutine flashRoutine;

    private void Awake()
    {
        playerNode = GetComponentInParent<Player>();
        playerInput = GetComponentInParent<PlayerInput>();
        flashlightAction = playerInput.actions.FindAction("Flashlight");

        if (lightBeamObject != null) lightBeamObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (flashlightAction != null) flashlightAction.performed += OnFlashlightInput;
    }

    private void OnDisable()
    {
        if (flashlightAction != null) flashlightAction.performed -= OnFlashlightInput;
    }

    private void Start()
    {
        SetState(FlashlightState.Ready);
        lightBeamObject.SetActive(false);
    }

    private void OnFlashlightInput(InputAction.CallbackContext context)
    {
        if (currentState == FlashlightState.Jammed)
        {
            playerNode.Audio.PlayFlashlightJammed();
            return;
        } 

        if (currentState == FlashlightState.On) return;

        if (playerNode.Health.isProtectedByBlanket || playerNode.Controller.isGliding) return;

        if (GameManager.Instance.hasFlashlight) 
        {
            if (playerNode != null) playerNode.Audio.PlayFlashlightOn();

            if (flashRoutine != null) StopCoroutine(flashRoutine);
            flashRoutine = StartCoroutine(UseLight());
        }
        else
        {
            if (playerNode != null) playerNode.Audio.PlayFlashlightJammed();
        }
    }

    private IEnumerator UseLight()
    {
        SetState(FlashlightState.On);
        if (lightBeamObject != null) lightBeamObject.SetActive(true);

        yield return new WaitForSeconds(activeDuration);

        StartCooldown();
    }

    public void InterruptFlashlight()
    {
        if (currentState == FlashlightState.On) StartCooldown();
    }

    private void StartCooldown()
    {
        StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        SetState(FlashlightState.Jammed);
        lightBeamObject.SetActive(false);

        yield return new WaitForSeconds(cooldownDuration);

        SetState(FlashlightState.Ready);
    }

    private void SetState(FlashlightState newState)
    {
        bool isStateChanging = currentState != newState;
        currentState = newState;

        if (feedbackRenderer != null)
        {
            switch (currentState)
            {
                case FlashlightState.Ready:
                    feedbackRenderer.color = readyColor;
                    if (isStateChanging && playerNode != null) playerNode.Audio.PlayFlashlightReady();
                    break;
                case FlashlightState.On:
                    feedbackRenderer.color = onColor;
                    break;
                case FlashlightState.Jammed:
                    feedbackRenderer.color = jammedColor;
                    if (isStateChanging && playerNode != null) playerNode.Audio.PlayFlashlightJammed();
                    break;    
            } 
        }
    }
}
