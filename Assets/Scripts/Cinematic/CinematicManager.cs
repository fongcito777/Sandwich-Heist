using UnityEngine;
using UnityEngine.Video;

public class CinematicManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public AudioSource videoAudioSource; // Audio del video (si est� separado)
    //public AudioClip cinematicAudio; // M�sica de fondo si es diferente

    public GameObject skipButton; // Bot�n para saltar la cinem�tica

    void Start()
    {
        // Pausa cualquier m�sica actual y reproduce el audio del video.
        AudioManager.Instance.StopMusic();
        // videoAudioSource.clip = cinematicAudio;
        videoAudioSource.Play();

        // Asocia el evento loopPointReached al m�todo EndCinematic
        videoPlayer.loopPointReached += EndCinematic;

        // Opcional: Desactivar el bot�n hasta que el video est� listo.
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
