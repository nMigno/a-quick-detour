using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SoundMenu : MonoBehaviour
{
    [Header("Audio Configuration")]
    public AudioMixer audioMixer;

    [Header("Sliders")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Sound feedback")]
    [Tooltip("AudioSource which is going to reproduce sound (GameManager's AudioSource)")]
    [SerializeField] private AudioSource uiAudioSource;
    [Tooltip("SFX when slider moves")]
    [SerializeField] private AudioClip sliderTickClip;
    [Tooltip("Minimum time between sounds to avoid MachineGun effect")]
    [SerializeField] private float soundCooldown = 0.05f;

    private float lastSoundTime = 0f;


    void Start()
    {
        float musicVol = GameManager.Instance.currentMusicVolume;
        float sfxVol = GameManager.Instance.currentSFXVolume;

        if (musicSlider != null) musicSlider.value = musicVol;
        if (sfxSlider != null) sfxSlider.value = sfxVol;

        if (audioMixer != null)
        {
            audioMixer.SetFloat("MusicVolume", ConvertToDecibels(musicVol));
            audioMixer.SetFloat("SFXVolume", ConvertToDecibels(sfxVol));
        }
    }

    public void SetMusicVolume(float sliderValue)
    {
        if (audioMixer != null)
        {
            audioMixer.SetFloat("MusicVolume", ConvertToDecibels(sliderValue));
        }

        GameManager.Instance.currentMusicVolume = sliderValue;
        PlayTickSound();
    }

    public void SetSFXVolume(float sliderValue)
    {
        if (audioMixer != null)
        {
            audioMixer.SetFloat("SFXVolume", ConvertToDecibels(sliderValue));
        }

        GameManager.Instance.currentSFXVolume = sliderValue;
        PlayTickSound();
    }

    private void PlayTickSound()
    {
        if (uiAudioSource != null && sliderTickClip != null)
        {
            // UnscaledTime for paused game (Time.timeScale = 0)
            if (Time.unscaledTime - lastSoundTime > soundCooldown)
            {
                uiAudioSource.PlayOneShot(sliderTickClip);
                lastSoundTime = Time.unscaledTime;
            }
        }
    }

    private float ConvertToDecibels(float sliderValue)
    {
        if (sliderValue <= 0.001f) return -80f; // Mute
        return Mathf.Log10(sliderValue) * 20f; // Logaritmic value
    }
}
