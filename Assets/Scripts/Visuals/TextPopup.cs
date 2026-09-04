using UnityEngine;
using System.Collections;
using TMPro;

public class TextPopup : MonoBehaviour
{
    [Header ("UI References")]
    [SerializeField] private TextMeshProUGUI popupText;

    [Header("Display Settings")]
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float displayDuration = 3f;

    public void ShowText(string message)
    {
        if (popupText != null) popupText.text = message;
        StartCoroutine(AnimatePopup());
    }

    private IEnumerator AnimatePopup()
    {
        SetTextAlpha(0f);

        float time = 0f;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            SetTextAlpha(Mathf.Lerp(0f, 1f, time / fadeDuration));
            yield return null;
        }
        SetTextAlpha(1f);

        yield return new WaitForSeconds(displayDuration);

        time = 0f;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            SetTextAlpha(Mathf.Lerp(1f, 0f, time / fadeDuration));
            yield return null;
        }
        SetTextAlpha(0f);

        Destroy(gameObject);
    }

    private void SetTextAlpha(float alpha)
    {
        if (popupText != null) popupText.alpha = alpha;
    }
}
