using UnityEngine;

public class MenuMusicManager : MonoBehaviour
{
    public AudioClip menuMusic; // Clip de la música del menú

    void Start()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusic(menuMusic, false); // Continúa donde quedó
        }
    }
}
