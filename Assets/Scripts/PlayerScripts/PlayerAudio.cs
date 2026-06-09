using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerAudio : MonoBehaviour
{
    private AudioSource audioSource;

    [Header("Movement")]
    public AudioClip jumpClip;
    public AudioClip moveClip;

    [Header("Flashlight")]
    public AudioClip flashlightOnClip;
    public AudioClip flashlightJammedClip;
    public AudioClip flashlightReadyClip;

    [Header("Combat")]
    public AudioClip hurtClip;
    public AudioClip deathClip;

    [Header("Pitch variation")]
    [SerializeField, Range(0.9f, 1.1f)] private float minPitch = 0.9f;
    [SerializeField, Range(0.9f, 1.1f)] private float maxPitch = 1.1f;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.pitch = Random.Range(minPitch, maxPitch);
            audioSource.PlayOneShot(clip);
        }
    }

    public void PlayJump() => PlaySound(jumpClip);
    public void PlayFlashlightOn() => PlaySound(flashlightOnClip);
    public void PlayFlashlightJammed() => PlaySound(flashlightJammedClip);
    public void PlayFlashlightReady() => PlaySound(flashlightReadyClip);
    public void PlayHurt() => PlaySound(hurtClip);
    public void PlayDeath() => PlaySound(deathClip);
}
