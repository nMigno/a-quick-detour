using Unity.VisualScripting;
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

    public void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        GetComponent<Collider2D>().isTrigger = true;
    }

    public void Interact(Player player)
    {
        if (isActivated) return;

        isActivated = true;
        
        if (checkpointSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(checkpointSound);
        }

        spriteRenderer.sprite = activeSprite;
        string currentScene = SceneManager.GetActiveScene().name;
        GameManager.Instance.SetCheckpoint(transform.position, currentScene);
        player.Health.HealToMax();

        Debug.Log($"Checkpoint saved at {currentScene}. Health restored!");
    }
}
