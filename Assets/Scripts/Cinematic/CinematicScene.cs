using UnityEngine;
using UnityEngine.Video;

public class CinematicScene : MonoBehaviour
{
    public AudioClip cinematicMusic; // Opcional, música específica para la cinemática
    public VideoPlayer videoPlayer;  // Video Player que contiene el MP4

    void Start()
    {
        // Detiene la música del menú
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopMusic();
        }

        // Opcional: Reproduce música específica si no tiene audio el MP4
        if (cinematicMusic != null)
        {
            AudioManager.Instance.PlayMusic(cinematicMusic);
        }

        // Reproducir video (configurado en el VideoPlayer)
        videoPlayer.Play();
    }
}
