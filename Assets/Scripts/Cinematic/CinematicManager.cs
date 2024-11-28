using UnityEngine;
using UnityEngine.Video;

public class CinematicManager : MonoBehaviour
{
    [SerializeField] private VideoClip videoClip;  // Reference from Assets/Videos
    [SerializeField] private string videoFileName;  // Name of file in StreamingAssets
    public VideoPlayer videoPlayer;
    public AudioSource videoAudioSource;
    public GameObject skipButton;

    void Start()
    {
        AudioManager.Instance.StopMusic();
        videoAudioSource.Play();

        #if UNITY_WEBGL && !UNITY_EDITOR
                    string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);
                    videoPlayer.url = videoPath;
        #else
                videoPlayer.clip = videoClip;
        #endif

        videoPlayer.loopPointReached += EndCinematic;
        skipButton.SetActive(true);
        videoPlayer.Play();
    }

    public void SkipCinematic()
    {
        videoPlayer.Stop();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Tutorial");
    }

    void EndCinematic(VideoPlayer vp)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Tutorial");
    }
}
