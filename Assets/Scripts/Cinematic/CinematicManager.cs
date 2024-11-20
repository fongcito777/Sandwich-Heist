using UnityEngine;
using UnityEngine.Video;

public class CinematicManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public AudioSource videoAudioSource; // Audio del video (si está separado)
    //public AudioClip cinematicAudio; // Música de fondo si es diferente

    public GameObject skipButton; // Botón para saltar la cinemática

    void Start()
    {
        // Pausa cualquier música actual y reproduce el audio del video.
        AudioManager.Instance.StopMusic();
        // videoAudioSource.clip = cinematicAudio;
        videoAudioSource.Play();

        // Asocia el evento loopPointReached al método EndCinematic
        videoPlayer.loopPointReached += EndCinematic;

        // Opcional: Desactivar el botón hasta que el video esté listo.
        skipButton.SetActive(true);
    }

    public void SkipCinematic()
    {
        videoPlayer.Stop();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Tutorial");
    }

    void EndCinematic(VideoPlayer vp)
    {
        // Cambia de escena al terminar el video
        UnityEngine.SceneManagement.SceneManager.LoadScene("Tutorial");
    }
}
