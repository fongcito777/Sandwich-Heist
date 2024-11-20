using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SpiceManager : MonoBehaviour
{
    // Tilemap
    [SerializeField] private Tilemap targetTilemap;
    [SerializeField] private TileBase tileToPaint;
    private Vector3Int previousTilePosition;
    private BoxCollider2D _bc;
    
    // UI
    public int ketchupCount = 0;
    [SerializeField] private TextMeshProUGUI _ketchupText;
    [SerializeField] private GameObject _ui;
    [SerializeField] private GameObject _spices;
    [SerializeField] private GameObject _endUi;
    [SerializeField] private GameObject _gameOverUi;
    [SerializeField] private TextMeshProUGUI rankText;

    public Button continueBtn_End;
    public Button retryBtn_End;
    public Button quitBtn_GameOver;
    public Button retryBtn_GameOver;

    [Header("Scene Names")]
    public string currentSceneName; // Nombre de la escena actual (opcional, puede obtenerse automáticamente)
    public string menuSceneName; // Nombre de la escena del menú principal

    private void Start()
    {
        _bc = GetComponent<BoxCollider2D>();

        // Asigna automáticamente el nombre de la escena actual si no se especifica
        if (string.IsNullOrEmpty(currentSceneName))
        {
            currentSceneName = SceneManager.GetActiveScene().name;
        }

        // Ensure these GameObjects are assigned in the Inspector
        if (_ketchupText == null || _spices == null || _ui == null || _endUi == null || _gameOverUi == null)
        {
            Debug.LogError("One or more UI elements are not assigned in the Inspector.");
            return;
        }
        
        _spices.SetActive(true);
        _endUi.SetActive(false);
        _gameOverUi.SetActive(false);

        continueBtn_End.onClick.AddListener(QuitToMenu);
        retryBtn_End.onClick.AddListener(RetryLevel);

        quitBtn_GameOver.onClick.AddListener(QuitToMenu);
        retryBtn_GameOver.onClick.AddListener(RetryLevel);
    }
    
    private void Update()
    {
        // UI
        _ketchupText.text = "KETCHUP: " + ketchupCount.ToString();
        
        // Get the current tile position
        var currentTilePosition = targetTilemap.WorldToCell(transform.position);
        
        // Only spurt if we've moved to a new tile
        if (currentTilePosition != previousTilePosition)
        {
            if (ketchupCount > 0)
            {
                targetTilemap.SetTile(currentTilePosition, tileToPaint);
                previousTilePosition = currentTilePosition;
                ketchupCount--;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Tomato"))
        {
            ketchupCount += 100;
            Destroy(other.gameObject);
        }

        if (other.gameObject.CompareTag("Police"))
        {
            _gameOverUi.SetActive(true); // Activa el nuevo panel de Game Over
            Time.timeScale = 0; // Pausa el juego
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("End"))
        {
            float floorCount = CountTiles(GameObject.FindGameObjectWithTag("Floor").GetComponent<Tilemap>());
            float ketchupTotal = CountTiles(GameObject.FindGameObjectWithTag("Ketchup").GetComponent<Tilemap>());
            var result = (ketchupTotal / floorCount) * 100;
            Debug.Log(result);
            
            _spices.SetActive(false);
            _ui.SetActive(true);
            _endUi.SetActive(true);

            if (result < 60)
            {
                rankText.text = "You got rank: D";
            }
            else if (result >= 60 && result < 70)
            {
                rankText.text = "You got rank: C";
            }
            else if (result >= 70 && result < 80)
            {
                rankText.text = "You got rank: B";
            }
            else if (result >= 80 && result < 90)
            {
                rankText.text = "You got rank: A";
            }
            else if (result >= 90)
            {
                rankText.text = "You got rank: S";
            }
            Time.timeScale = 0; // Pausa el juego

        }
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


    int CountTiles(Tilemap tilemap)  // For counting tiles of the current map
    {
        int count = 0;
        BoundsInt bounds = tilemap.cellBounds;

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
            {
                count++;
            }
        }

        return count;
    }
}
