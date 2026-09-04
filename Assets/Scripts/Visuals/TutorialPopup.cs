using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;

[RequireComponent(typeof(Collider2D))]
public class TutorialPopup : MonoBehaviour
{
    [Header("UI Settings")]
    [Tooltip("Canvas of visual tutorial")]
    [SerializeField] private GameObject popupVisuals;
    [SerializeField] private TextMeshProUGUI popupText;

    [Header("Text Format")]
    [Tooltip("e.g.: '{0} for flashlight' or ' {0} + {1} '")]
    [SerializeField] private string textFormat = "{0}";

    [Tooltip("Input System Actions names. E.g.: Element 0: 'Jump', Element 1: 'Blanket' ")]
    [SerializeField] private string[] actionNames;

    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 0.2f;

    private PlayerInput playerInput;
    private Coroutine fadeCoroutine;

    private void Start()
    {
        if (popupVisuals != null) popupVisuals.SetActive(false);

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerInput = playerObj.GetComponent<PlayerInput>();
    }

    private void UpdatePopupText()
    {
        if (playerInput == null || popupText == null || actionNames.Length == 0) return;

        string[] bindings = new string[actionNames.Length];
        int schemeIndex = playerInput.currentControlScheme == "Gamepad" ? 1 : 0;

        for (int i = 0; i < actionNames.Length; i++)
        {
            InputAction action = playerInput.actions.FindAction(actionNames[i]);
            if (action != null)
            {
                bindings[i] = action.GetBindingDisplayString(schemeIndex, InputBinding.DisplayStringOptions.DontIncludeInteractions);
            }
            else
            {
                bindings[i] = "?";
            }
        }

        popupText.text = string.Format(textFormat, bindings);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && popupVisuals != null)
        {
            if (playerInput == null) playerInput = other.GetComponent<PlayerInput>();

            UpdatePopupText();

            if (!popupVisuals.activeSelf)
            {
                popupVisuals.SetActive(true);
                SetTextAlpha(0f);
            }

            FadeTextTo(1f);
        }
    }
        

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && popupVisuals != null && popupVisuals.activeSelf)
        {
            FadeTextTo(0f);
        }
    }

    private void FadeTextTo(float targetAlpha)
    {
        if (popupText == null) return;
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeRoutine(targetAlpha));
    }

    private IEnumerator FadeRoutine(float targetAlpha)
    {
        float startAlpha = popupText.color.a;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            SetTextAlpha(alpha);
            yield return null;
        }

        SetTextAlpha(targetAlpha);
        if (targetAlpha == 0f && popupVisuals != null)
        {
            popupVisuals.SetActive(false);
        }
    }

    private void SetTextAlpha(float alpha)
    {
        if (popupText != null) popupText.alpha = alpha;
    }
}
