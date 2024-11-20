using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip menuMusic;
    public AudioClip levelMusic;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void PlayMusic(AudioClip clip, bool restart = true)
    {
        if (musicSource.clip != clip || restart)
        {
            musicSource.clip = clip;
            musicSource.Play();
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    // Ajustar el volumen de la música
    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    // Ajustar el volumen de los SFX
    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }

    public void PauseMusic()
    {
        musicSource.Pause(); // Pausa la música actual
    }

    public void ResumeMusic()
    {
        musicSource.UnPause(); // Reanuda la música actual
    }
}
