using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class Door : MonoBehaviour
{
    [Header("Door Settings")]
    public string myDoorID;
    public string sceneToLoad;
    public string destinationDoorID;

    [Header("Lock Settings")]
    public bool isLocked = false;

    [Header("Endgame")]
    public bool isFinalDoor = false;

    [Header("UI Interaction Settings")]
    [Tooltip("Canvas/GameObject that will appear over door")]
    [SerializeField] private GameObject interactPopup;
    [Tooltip("Text component for dynamic bind change")]
    [SerializeField] private TextMeshProUGUI interactText;

    [Header("Fade Settings")]
    [Tooltip("Fade-In / Fade-Out durations (seconds)")]
    [SerializeField] private float fadeDuration = 0.1f;

    [Header("Audio")]
    public AudioClip openSound;
    private AudioSource audioSource;

    private bool playerInRange = false;
    private Coroutine fadeCoroutine;

    private PlayerInput playerInput;
    private Collider2D playerCollider;
    private Rigidbody2D playerRb;

    public Transform playerSpawn;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (interactPopup != null) interactPopup.SetActive(false);
    }

    private void Start()
    {
        GameObject playerObj = GameObject.Find("Player");
        playerInput = playerObj.GetComponent<PlayerInput>();        
        playerCollider = playerObj.GetComponent<Collider2D>();
        playerRb = playerObj.GetComponent<Rigidbody2D>();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (playerInRange && context.started)
        {
            if (isLocked || (!isFinalDoor && string.IsNullOrEmpty(sceneToLoad)))
            {
                Debug.Log("Door locked.");
                return;
            }

            StartCoroutine(TryInteract());            
        }
    }

    private IEnumerator TryInteract()
    {
        playerInput.DeactivateInput();
        playerCollider.enabled = false;
        playerRb.linearVelocity = Vector2.zero;
        playerRb.constraints = RigidbodyConstraints2D.FreezeAll;

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        if (interactPopup != null) interactPopup.SetActive(false);

        if (openSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(openSound);
        }
        
        yield return new WaitForSeconds(0.2f);

        // Fade screen transition
        LevelTransition transition = FindAnyObjectByType<LevelTransition>();

        if (transition != null) 
        {
            if (isFinalDoor)
            {
                transition.TriggerDemoEndFade(Transition);
            }
            else
            {
                transition.TriggerDoorFade(Transition); // Callback once screen is full black
            }
        }
        else
        {
            Transition(); // Fallback if no transition prefab loaded
        }
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

        playerCollider.enabled = true;
        playerRb.constraints = RigidbodyConstraints2D.FreezeRotation;

        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogWarning("Transition aborted: sceneToLoad is empty in Inspector");
            return;
        }

        Debug.Log($"Travelling to {sceneToLoad}, destination: {destinationDoorID}");
        GameManager.Instance.UseDoorTransition(sceneToLoad, destinationDoorID);
    }

    private void UpdatePopupText()
    {
        if (interactPopup == null || playerInput == null) return;

        InputAction interactAction = playerInput.actions.FindAction("Interactions");

        if (interactAction != null)
        {
            // Index 0 = KB. Index 1 = Gamepad
            int bindingIndex = playerInput.currentControlScheme == "Gamepad" ? 1 : 0;
            string keyName = interactAction.GetBindingDisplayString(bindingIndex,
            InputBinding.DisplayStringOptions.DontIncludeInteractions);

            keyName = keyName.Replace("D-Pad ", "")
                             .Replace("DPAD ", "")
                             .Replace("Left Stick ", "")
                             .Replace("Right Stick ", "");

            interactText.text = $"{keyName.ToUpper()}";
        }
    }

    private void FadeTextTo(float targetAlpha)
    {
        if (interactText == null) return;

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeRoutine(targetAlpha));
    }

    private IEnumerator FadeRoutine(float targetAlpha)
    {
        float startAlpha = interactText.color.a;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            SetTextAlpha(alpha);
            yield return null;
        }

        SetTextAlpha(targetAlpha);

        if(targetAlpha == 0f && interactPopup != null)
        {
            interactPopup.SetActive(false);
        }
    }

    private void SetTextAlpha(float alpha)
    {
        Color c = interactText.color;
        c.a = alpha;
        interactText.color = c;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (interactPopup != null && !isLocked)
            {
                UpdatePopupText();

                if (!interactPopup.activeSelf)
                {
                    SetTextAlpha(0f);
                    interactPopup.SetActive(true);
                }

                FadeTextTo(1f); // Starts fade in
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (interactPopup != null && interactPopup.activeSelf)
            {
                FadeTextTo(0f); // Starts fade out
            } 
        }
    }
}
