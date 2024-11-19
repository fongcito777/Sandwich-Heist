using UnityEngine;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    public GameObject[] levelContents;  // Arreglo con los niveles (título, imagen, botón).
    public Button leftButton;  // Flecha izquierda
    public Button rightButton; // Flecha derecha
    private int currentIndex = 0;  // Índice del nivel actual

    void Start()
    {
        leftButton.onClick.AddListener(PreviousLevel);
        rightButton.onClick.AddListener(NextLevel);

        UpdateLevelContent();
    }

    // Mover al nivel anterior
    void PreviousLevel()
    {
        currentIndex--;
        if (currentIndex < 0) currentIndex = levelContents.Length - 1;  // Volver al último nivel
        UpdateLevelContent();
    }

    // Mover al siguiente nivel
    void NextLevel()
    {
        currentIndex++;
        if (currentIndex >= levelContents.Length) currentIndex = 0;  // Volver al primer nivel
        UpdateLevelContent();
    }

    // Actualizar el contenido del nivel mostrado
    void UpdateLevelContent()
    {
        // Desactivar todos los niveles
        foreach (GameObject level in levelContents)
        {
            level.SetActive(false);
        }

        // Activar el nivel actual
        levelContents[currentIndex].SetActive(true);
    }
}
