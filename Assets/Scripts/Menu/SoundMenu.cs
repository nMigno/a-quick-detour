using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundMenu : MonoBehaviour
{
    public AudioMixer audioMixer;

    [Header("Sliders")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    void Start()
    {
        float musicVol = GameManager.Instance.currentMusicVolume;
        float sfxVol = GameManager.Instance.currentSFXVolume;

        musicSlider.value = musicVol;
        sfxSlider.value = sfxVol;

        audioMixer.SetFloat("MusicVolume", ConvertToDecibels(musicVol));
        audioMixer.SetFloat("SFXVolume", ConvertToDecibels(sfxVol));
    }

    public void SetMusicVolume(float sliderValue)
    {
        audioMixer.SetFloat("MusicVolume", ConvertToDecibels(sliderValue));
        // GM.Instance maintains changes onto other scenes
        GameManager.Instance.currentMusicVolume = sliderValue;
    }

    public void SetSFXVolume(float sliderValue)
    {
        audioMixer.SetFloat("SFXVolume", ConvertToDecibels(sliderValue));
        // GM.Instance maintains changes onto other scenes
        GameManager.Instance.currentSFXVolume = sliderValue;
    }

    float ConvertToDecibels(float sliderValue)
    {
        if (sliderValue == 0) return -80f; //Silence

        float linearValue = sliderValue / 10f;
        return Mathf.Log10(linearValue) * 20f;
    }
}
