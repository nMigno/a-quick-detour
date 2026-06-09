using UnityEngine;
using UnityEngine.UI;

public class SoundMenu : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    void Start()
    {
        musicSlider.value = GameManager.Instance.currentMusicVolume;
        sfxSlider.value = GameManager.Instance.currentSFXVolume;
    }

    public void SetMusicVolume(float sliderValue)
    {
        GameManager.Instance.currentMusicVolume = sliderValue;
    }

    public void SetSFXVolume(float sliderValue)
    {
        GameManager.Instance.currentSFXVolume = sliderValue;
    }
}
