using UnityEngine;
using UnityEngine.UI;

public class ExitGameUI : MonoBehaviour
{
    public GameObject confirmationPanel; // Asigna aquí el Panel de confirmación desde el Inspector
    public Button quitButton;
    public Button yesButton;
    public Button noButton;

    void Start()
    {
        confirmationPanel.SetActive(false); // Asegúrate de que la ventana esté oculta al inicio
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
        // Este código solo funciona en una aplicación ejecutable
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Solo para probar en el editor
#endif
    }
}
