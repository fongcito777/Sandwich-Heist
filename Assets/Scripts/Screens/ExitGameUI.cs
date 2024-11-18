using UnityEngine;
using UnityEngine.UI;

public class ExitGameUI : MonoBehaviour
{
    public GameObject confirmationPanel; // Asigna aqu� el Panel de confirmaci�n desde el Inspector
    public Button quitButton;
    public Button yesButton;
    public Button noButton;

    void Start()
    {
        confirmationPanel.SetActive(false); // Aseg�rate de que la ventana est� oculta al inicio
        quitButton.onClick.AddListener(ShowConfirmation);
        yesButton.onClick.AddListener(QuitGame);
        noButton.onClick.AddListener(HideConfirmation);
    }

    void ShowConfirmation()
    {
        confirmationPanel.SetActive(true);
    }

    void HideConfirmation()
    {
        confirmationPanel.SetActive(false);
    }

    void QuitGame()
    {
        // Este c�digo solo funciona en una aplicaci�n ejecutable
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Solo para probar en el editor
#endif
    }
}
