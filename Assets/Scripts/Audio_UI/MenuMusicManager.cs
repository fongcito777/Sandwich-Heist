using UnityEngine;

public class MenuMusicManager : MonoBehaviour
{
    public AudioClip menuMusic; // Clip de la m�sica del men�

    void Start()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusic(menuMusic, false); // Contin�a donde qued�
        }
    }
}
