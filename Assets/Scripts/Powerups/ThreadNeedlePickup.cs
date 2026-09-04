using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class ThreadNeedlePickup : MonoBehaviour
{
    [Header("Level Design Elements")]
    [Tooltip("Objects/Walls that disappear once picked up")]
    [SerializeField] private GameObject[] platformsToDisable;
    [Tooltip("Objects/Walls that appear once picked up")]
    [SerializeField] private GameObject[] objectsToEnable;

    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 1f;

    [Header("Text Event Settings")]
    [Tooltip("Canvas prefab with TextPopup script")]
    [SerializeField] private GameObject textPopupPrefab;

    [Tooltip("Text to display when picked up")]
    [TextArea(2, 4)]
    [SerializeField] private string popupMessage = "Blanket repaired!";

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.hasBlanketBase)
        {
            foreach (GameObject obj in platformsToDisable)
            {
                if (obj != null) obj.SetActive(false);
            }

            foreach (GameObject obj in objectsToEnable)
            {
                if (obj != null)
                {
                    obj.SetActive(true);
                    SpriteRenderer[] renderers = obj.GetComponentsInChildren<SpriteRenderer>(true);
                    foreach (SpriteRenderer sr in renderers)
                    {
                        Color c = sr.color;
                        c.a = 1f;
                        sr.color = c;
                    }
                }
            }

            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if(GameManager.Instance != null)
            {
                GameManager.Instance.UnlockBlanketBase();
                GameManager.Instance.UnlockBlanketGlider();
            }

            Player player = other.GetComponent<Player>();
            if (player != null && player.PlayerBlanket != null)
            {
                player.PlayerBlanket.enabled = true;
            }

            Debug.Log("Blanket repaired!");

            if (textPopupPrefab != null)
            {
                GameObject popupInstance = Instantiate(textPopupPrefab);
                TextPopup popupScript = popupInstance.GetComponent<TextPopup>();

                if (popupScript != null) popupScript.ShowText(popupMessage);
            }

            StartCoroutine(FadeSequence());

            GetComponent<Collider2D>().enabled = false;
            GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    private IEnumerator FadeSequence()
    {
        List<SpriteRenderer> spritesToDisable = new List<SpriteRenderer>();
        foreach (GameObject obj in platformsToDisable)
        {
            if (obj != null) spritesToDisable.AddRange(obj.GetComponentsInChildren<SpriteRenderer>(true));
        }

        List<SpriteRenderer> spritesToEnable = new List<SpriteRenderer>();
        foreach (GameObject obj in objectsToEnable)
        {
            if (obj != null)
            {
                obj.SetActive(true);

                SpriteRenderer[] renderers = obj.GetComponentsInChildren<SpriteRenderer>(true);
                foreach (SpriteRenderer sr in renderers)
                {
                    Color c = sr.color;
                    c.a = 0f;
                    sr.color = c;
                    spritesToEnable.Add(sr);
                }
            }
        }

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;

            foreach (SpriteRenderer sr in spritesToDisable)
            {
                if (sr != null)
                {
                    Color c = sr.color;
                    c.a = Mathf.Lerp(1f, 0f, t);
                    sr.color = c;
                }
            }

            foreach (SpriteRenderer sr in spritesToEnable)
            {
                if (sr != null)
                {
                    Color c = sr.color;
                    c.a = Mathf.Lerp(0f, 1f, t);
                    sr.color = c;
                }
            }

            yield return null;
        }

        foreach (GameObject obj in platformsToDisable)
        {
            if (obj != null) obj.SetActive(false);
        }

        gameObject.SetActive(false);
    }
}
