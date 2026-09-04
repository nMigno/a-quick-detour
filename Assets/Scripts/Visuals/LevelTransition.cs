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
            fadeImage.color = new Color(0f, 0f, 0f, 1f);
            StartCoroutine(StartFadeInDelay());
        }
        else
        {
            Debug.LogWarning("LevelTransition: Fade Image not assigned on Inspector");
        }
    }

    private IEnumerator StartFadeInDelay()
    {
        yield return null;
        yield return null;
        StartCoroutine(Fade(1f, 0f));
    }

    public void TriggerDoorFade(System.Action onFadeComplete, Color fadeColor = default)
    {
        if (!isTransitioning && fadeImage != null)
        {
            isTransitioning = true;
            fadeImage.gameObject.SetActive(true);

            if (fadeColor == default) fadeColor = Color.black;

            fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0f);
            GameManager.Instance.FadeOutMusic(fadeDuration);

            StartCoroutine(Fade(0f, 1f, onFadeComplete));
        }
        else
        {
            onFadeComplete?.Invoke(); // If no image reference, executes transition immediately
        }
    }

    public void TriggerDemoEndFade(System.Action onFadeComplete)
    {
        if (!isTransitioning && fadeImage != null)
        {
            isTransitioning = true;
            fadeImage.gameObject.SetActive(true);

            GameManager.Instance.FadeOutMusic(1f);

            StartCoroutine(DemoEndSequence(onFadeComplete));
        }
        else
        {
            onFadeComplete?.Invoke();
        }
    }

    private IEnumerator DemoEndSequence(System.Action onFadeComplete)
    {
        fadeImage.color = new Color(1f, 1f, 1f, 0f);
        yield return StartCoroutine(FadeRoutine(0f, 1f, 1f));

        float elapsed = 0f;
        while (elapsed < 0.5f)
        {
            elapsed += Time.unscaledDeltaTime;
            fadeImage.color = Color.Lerp(Color.white, Color.black, elapsed / 0.5f);
            yield return null;
        }
        fadeImage.color = Color.black;

        onFadeComplete?.Invoke();

        yield return new WaitForSecondsRealtime(0.5f);

        yield return StartCoroutine(FadeRoutine(1f, 0f, 0.5f));

        isTransitioning = false;
    }

    private IEnumerator Fade(float startAlpha, float targetAlpha, System.Action onComplete = null)
    {
        yield return StartCoroutine(FadeRoutine(startAlpha, targetAlpha, fadeDuration));
        onComplete?.Invoke();
        isTransitioning = false;
    }

    private IEnumerator FadeRoutine(float startAlpha, float targetAlpha, float duration)
    {
        float elapsed = 0f;
        Color color = fadeImage.color;

        while (elapsed < duration)
        {
            float safeDeltaTime = Mathf.Min(Time.unscaledDeltaTime, 0.1f);
            elapsed += safeDeltaTime;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            fadeImage.color = color;
            yield return null;
        }
        
        color.a = targetAlpha;
        fadeImage.color = color;
    }
}
