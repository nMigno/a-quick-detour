using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerFootsteps : MonoBehaviour
{
    [Header("Sounds Settings")]
    [SerializeField] private AudioClip[] footstepClips;

    [Header("Pitch Settings")]
    [Range(0.8f, 1.2f)] [SerializeField] private float minPitch = 0.95f;
    [Range(0.8f, 1.2f)] [SerializeField] private float maxPitch = 1.05f;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayFootstepSound()
    {
        if (footstepClips != null || footstepClips.Length == 0) return;

        int randomIndex = Random.Range(0, footstepClips.Length);
        AudioClip randomClip = footstepClips[randomIndex];

        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.PlayOneShot(randomClip);
    }
}
