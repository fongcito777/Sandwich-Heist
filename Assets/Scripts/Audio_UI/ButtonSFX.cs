using UnityEngine;
using UnityEngine.UI;

public class ButtonSFX : MonoBehaviour
{
    public AudioClip buttonClickClip; // Arrastra el clip para el bot�n en el Inspector.

    public void PlayClickSFX()
    {
        if (buttonClickClip != null)
        {
            AudioManager.Instance.PlaySFX(buttonClickClip);
        }
    }
}
