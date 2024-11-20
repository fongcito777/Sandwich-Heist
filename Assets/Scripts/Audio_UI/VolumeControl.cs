using UnityEngine;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;

    void Start()
    {
        // Inicializa los sliders con valores guardados o predeterminados
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.75f);

        // Aplica los valores iniciales
        AudioManager.Instance.SetMusicVolume(musicSlider.value);
        AudioManager.Instance.SetSFXVolume(sfxSlider.value);

        // Escucha los cambios en los sliders
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMusicVolume(float volume)
    {
        AudioManager.Instance.SetMusicVolume(volume);
        PlayerPrefs.SetFloat("MusicVolume", volume); // Guarda el volumen
    }

    public void SetSFXVolume(float volume)
    {
        AudioManager.Instance.SetSFXVolume(volume);
        PlayerPrefs.SetFloat("SFXVolume", volume); // Guarda el volumen
    }
}
