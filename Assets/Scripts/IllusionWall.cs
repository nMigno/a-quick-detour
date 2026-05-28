using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class IllusionWall : MonoBehaviour
{
    public string wallID;

    [Header("Visual Settings")]
    [Tooltip("Opacity when wall is illuminated")]
    [Range(0f, 1f)] public float revealedAlpha = 0.2f;

    private SpriteRenderer spriteRenderer;
    private Collider2D wallCollider;
    private bool isRevealed = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        wallCollider = GetComponent<Collider2D>();
    }

    private void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.revealedIllusionWalls.Contains(wallID))
        {
            ApplyRevealedVisuals();
            isRevealed = true;
        }
    }

    public void Reveal()
    {
        if (isRevealed || !GameManager.Instance.hasFlashlightUpgrade) return;

        isRevealed = true;
        wallCollider.enabled = false;

        Color revealColor = spriteRenderer.color;
        revealColor.a = revealedAlpha;
        spriteRenderer.color = revealColor;

        GameManager.Instance.RegisterRevealedWall(wallID);

        Debug.Log("Illusion wall dissapeared!");
    }

    private void ApplyRevealedVisuals()
    {
        wallCollider.enabled = false;
        gameObject.SetActive(false);
    }
}
