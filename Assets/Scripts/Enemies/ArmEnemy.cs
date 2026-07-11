using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class ArmEnemy : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private int damageAmount = 1;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip damageClip;


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHP playerHP = other.GetComponent<PlayerHP>();

            if(playerHP.TakeDamage(damageAmount, transform.position))
            {
                if (audioSource != null && damageClip != null)
                {
                    audioSource.pitch = Random.Range(0.9f, 1.1f);
                    audioSource.PlayOneShot(damageClip);
                }
            }        
        }
    }
}
