using UnityEngine;

[RequireComponent(typeof(BoxCollider2D), typeof(AudioSource), typeof(Rigidbody2D))]
public class FinalBoss : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 2f;
    private bool isActive = false;

    [Header("Damage Settings")]
    [SerializeField] private int instakillDamage = 99;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip spawnSound;
    [SerializeField] private AudioClip hitPlayerSound;
    [SerializeField] private AudioClip[] ambientSounds;
    [SerializeField] private float minSoundInterval = 3f;
    [SerializeField] private float maxSoundInterval = 8f;

    private AudioSource audioSource;
    private Rigidbody2D rb;
    private float nextSoundTimer;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!isActive) return;
        HandleAmbientSounds();
    }

    private void FixedUpdate()
    {
        if (!isActive)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        rb.linearVelocity = new Vector2(speed, rb.linearVelocity.y);
    }

    public void ActivateBoss()
    {
        if (isActive) return;

        gameObject.SetActive(true);
        isActive = true;

        if (spawnSound != null) audioSource.PlayOneShot(spawnSound);

        SetNextAudioTimer();

        Debug.Log("Final Boss Activated!");
    }

    private void HandleAmbientSounds()
    {
        if (ambientSounds == null || ambientSounds.Length == 0) return;

        nextSoundTimer -= Time.deltaTime;

        if (nextSoundTimer <= 0f)
        {
            AudioClip randomClip = ambientSounds[Random.Range(0, ambientSounds.Length)];
            audioSource.PlayOneShot(randomClip);
            SetNextAudioTimer();
        }
    }

    private void SetNextAudioTimer()
    {
        nextSoundTimer = Random.Range(minSoundInterval, maxSoundInterval);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();

        if (player != null)
        {
            player.Health.isProtectedByBlanket = false;

            if (player.Health.TakeDamage(instakillDamage, transform.position))
            {
                if (hitPlayerSound != null) audioSource.PlayOneShot(hitPlayerSound);
            }
        }
    }
}
