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
    }

    public void SetSFXVolume(float sliderValue)
    {
        if (audioMixer != null)
        {
            audioMixer.SetFloat("SFXVolume", ConvertToDecibels(sliderValue));
        }

        GameManager.Instance.currentSFXVolume = sliderValue;
    }

    private float ConvertToDecibels(float sliderValue)
    {
        if (sliderValue <= 0.001f) return -80f; // Mute
        return Mathf.Log10(sliderValue) * 20f; // Logaritmic value
    }
}
