using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(AudioSource))]
public class Checkpoint : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite activeSprite;

    [Header("Audio")]
    public AudioClip checkpointSound;

    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    private bool isActivated = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isActivated || !other.CompareTag("Player")) return;

        Player player = other.GetComponent<Player>();
        if (player != null) ActivateCheckpoint(player);
    }

    private void ActivateCheckpoint(Player player)
    {
        isActivated = true;

        if (checkpointSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(checkpointSound);
        }

        if (activeSprite != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = activeSprite;
        }

        string currentScene = SceneManager.GetActiveScene().name;
        GameManager.Instance.SetCheckpoint(transform.position, currentScene);
        player.Health.HealToMax();

        Debug.Log($"Checkpoint saved at {currentScene}. Health restored.");
    }
}
