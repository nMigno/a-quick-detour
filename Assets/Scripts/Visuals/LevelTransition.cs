using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class LevelTransition : MonoBehaviour
{
    [Header("Transition Settings")]
    [SerializeField] private string nextSceneName;
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isTransitioning && other.CompareTag("Player"))
        {
            isTransitioning = true;

            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null) player.canMove = false;

            StartCoroutine(Fade(0f, 1f, LoadNextLevel));
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

    private void LoadNextLevel()
    {
        GameManager.Instance.lastCheckpointPosition = Vector2.zero;
        GameManager.Instance.lastCheckpointScene = nextSceneName;
        SceneManager.LoadScene(nextSceneName);
    }
}
