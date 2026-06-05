using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class Checkpoint : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite activeSprite;

    private SpriteRenderer spriteRenderer;

    public void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        GetComponent<Collider2D>().isTrigger = true;
    }

    public void Interact(Player player)
    {
        spriteRenderer.sprite = activeSprite;
        string currentScene = SceneManager.GetActiveScene().name;
        GameManager.Instance.SetCheckpoint(transform.position, currentScene);
        player.Health.HealToMax();

        Debug.Log($"Checkpoint saved at {currentScene}. Health restored!");
    }
}
