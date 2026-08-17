using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelTransition : MonoBehaviour
{
    [Header("Transition Settings")]
    [SerializeField] private float fadeDuration = 0.5f;

    [Header("UI References")]
    [SerializeField] private Image fadeImage;

    private bool isTransitioning = false;

    private void Start()
    {
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            StartCoroutine(Fade(1f, 0f));
        }
        else
        {
            Debug.LogWarning("LevelTransition: Fade Image not assigned on Inspector");
        }
    }

    public void TriggerDoorFade(System.Action onFadeComplete)
    {
        if (!isTransitioning && fadeImage != null)
        {
            isTransitioning = true;
            fadeImage.gameObject.SetActive(true);
            StartCoroutine(Fade(0f, 1f, onFadeComplete));
        }
        else
        {
            onFadeComplete?.Invoke(); // If no image reference, executes transition immediately
        }
    }

    private IEnumerator Fade(float startAlpha, float targetAlpha, System.Action onComplete = null)
    {
        float elapsed = 0f;
        Color color = fadeImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }
        
        color.a = targetAlpha;
        fadeImage.color = color;
        onComplete?.Invoke();
    }
}
