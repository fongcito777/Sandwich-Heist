using UnityEngine;
using UnityEngine.Video;

public class CinematicScene : MonoBehaviour
{
    public AudioClip cinematicMusic; // Opcional, m�sica espec�fica para la cinem�tica
    public VideoPlayer videoPlayer;  // Video Player que contiene el MP4

    void Start()
    {
        // Detiene la m�sica del men�
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopMusic();
        }

        // Opcional: Reproduce m�sica espec�fica si no tiene audio el MP4
        if (cinematicMusic != null)
        {
            AudioManager.Instance.PlayMusic(cinematicMusic);
        }

        // Reproducir video (configurado en el VideoPlayer)
        videoPlayer.Play();
    }
}
