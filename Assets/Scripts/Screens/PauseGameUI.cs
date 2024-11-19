using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class PauseGameUI : MonoBehaviour
{
    public GameObject pausePanel; // Asigna el Panel de pausa en el Inspector
    public Button pauseButton;
    public Button resumeButton;
    public Button retryButton;
    public Button quitButton;
    public TextMeshProUGUI countdownText; // Agrega un Text en el Panel de pausa para mostrar la cuenta regresiva

    [Header("Scene Names")]
    public string currentSceneName; // Nombre de la escena actual (opcional, puede obtenerse automáticamente)
    public string menuSceneName; // Nombre de la escena del menú principal

    void Start()
    {
        // Asigna automáticamente el nombre de la escena actual si no se especifica
        if (string.IsNullOrEmpty(currentSceneName))
        {
            currentSceneName = SceneManager.GetActiveScene().name;
        }

        pausePanel.SetActive(false);

        pauseButton.onClick.AddListener(PauseGame);
        resumeButton.onClick.AddListener(() => StartCoroutine(ResumeGame()));
        retryButton.onClick.AddListener(RetryLevel);
        quitButton.onClick.AddListener(QuitToMenu);
    }

    void PauseGame()
    {
        Time.timeScale = 0; // Pausa el juego
        pausePanel.SetActive(true); // Muestra el panel de pausa
    }

    IEnumerator ResumeGame()
    {
        pausePanel.SetActive(false); // Oculta el panel de pausa
        countdownText.gameObject.SetActive(true);
        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSecondsRealtime(1); // Espera en tiempo real para que no se vea afectado por la pausa
        }

        countdownText.text = ""; // Limpia el texto de la cuenta regresiva
        countdownText.gameObject.SetActive(false);
        Time.timeScale = 1; // Reanuda el juego
    }

    void RetryLevel()
    {
        Time.timeScale = 1; // Restaura el tiempo antes de recargar la escena
        SceneManager.LoadScene(currentSceneName); // Recarga la escena actual
    }

    void QuitToMenu()
    {
        Time.timeScale = 1; // Restaura el tiempo antes de cambiar de escena
        SceneManager.LoadScene(menuSceneName); // Carga la escena del menú principal
    }
}
