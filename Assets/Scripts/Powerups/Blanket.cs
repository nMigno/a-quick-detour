using UnityEngine;

public class Blanket : MonoBehaviour
{
    [Header("Blanket Settings")]
    public Sprite blanketSprite;

    private Sprite originalSprite;
    private Player playerNode;
    private SpriteRenderer playerSpriteRenderer;
    private Animator anim;

    private void Awake()
    {
        playerNode = GetComponentInParent<Player>();

        if (playerNode != null)
        {
            anim = playerNode.GetComponent<Animator>();
            playerSpriteRenderer = playerNode.GetComponent<SpriteRenderer>();
        }
    }

    private void Start()
    {
        if (playerSpriteRenderer != null)
        {
            originalSprite = playerSpriteRenderer.sprite;
        }
    }

    public void ActivateBlanket(bool isActive)
    {
        if (playerNode == null) return;

        playerNode.Health.isProtectedByBlanket = isActive;
        playerNode.Controller.canMove = !isActive;

        if (playerNode.PlayerFlashlight != null && playerNode.PlayerFlashlight.feedbackRenderer != null)
        {
            playerNode.PlayerFlashlight.feedbackRenderer.enabled = !isActive;
        }

        if (playerNode.Audio != null)
        {
            if (isActive) playerNode.Audio.PlayBlanketOn();
            else playerNode.Audio.PlayBlanketOff();
        }

        if (anim != null) anim.SetBool("MantaOn", isActive);
    }
}
