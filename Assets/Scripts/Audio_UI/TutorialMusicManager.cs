using UnityEngine;

public class TutorialMusicManager : MonoBehaviour
{
    public AudioClip tutorialMusic;

    void Start()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusic(tutorialMusic);
        }
    }
}