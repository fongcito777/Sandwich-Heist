using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SpiceManager : MonoBehaviour
{
    // Tilemap
    [SerializeField] private Tilemap ketchupTilemap;
    [SerializeField] private TileBase tileToPaint;
    private Vector3Int _previousTilePosition;
    private BoxCollider2D _bc;
    private bool _isTouchingKetchup;
    
    // UI
    public int ketchupCount = 0;
    [SerializeField] private TextMeshProUGUI ketchupText;
    [SerializeField] private GameObject ui;
    [SerializeField] private GameObject spices;
    [SerializeField] private GameObject endUi;
    [SerializeField] private GameObject gameOverUi;
    [SerializeField] private TextMeshProUGUI rankText;

    [SerializeField] private Button continueBtnEnd;
    [SerializeField] private Button retryBtnEnd;
    [SerializeField] private Button quitBtnGameOver;
    [SerializeField] private Button retryBtnGameOver;

    [Header("Scene Names")]
    public string currentSceneName; // Nombre de la escena actual (opcional, puede obtenerse autom�ticamente)
    public string menuSceneName; // Nombre de la escena del men� principal

    private void Start()
    {
        _bc = GetComponent<BoxCollider2D>();

        // Asigna autom�ticamente el nombre de la escena actual si no se especifica
        if (string.IsNullOrEmpty(currentSceneName))
        {
            currentSceneName = SceneManager.GetActiveScene().name;
        }

        // Ensure these GameObjects are assigned in the Inspector
        if (ketchupText == null || spices == null || ui == null || endUi == null || gameOverUi == null)
        {
            Debug.LogError("One or more UI elements are not assigned in the Inspector.");
            return;
        }
        
        spices.SetActive(true);
        endUi.SetActive(false);
        gameOverUi.SetActive(false);

        continueBtnEnd.onClick.AddListener(QuitToMenu);
        retryBtnEnd.onClick.AddListener(RetryLevel);

        quitBtnGameOver.onClick.AddListener(QuitToMenu);
        retryBtnGameOver.onClick.AddListener(RetryLevel);
    }
    
    private void Update()
    {
        // UI
        ketchupText.text = "KETCHUP: " + ketchupCount.ToString();
        
        // Get the current tile position
        var currentTilePosition = ketchupTilemap.WorldToCell(transform.position);
        
        // Only spurt if we've moved to a new tile
        if (currentTilePosition != _previousTilePosition)
        {
            if (ketchupCount > 0)
            {
                if (!_isTouchingKetchup)
                {
                    ketchupTilemap.SetTile(currentTilePosition, tileToPaint);
                    _previousTilePosition = currentTilePosition;
                    ketchupCount--;
                }
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
            gameOverUi.SetActive(true); // Activa el nuevo panel de Game Over
            Time.timeScale = 0; // Pausa el juego
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ketchup")) _isTouchingKetchup = true;
        
        if (other.gameObject.CompareTag("End"))
        {
            float floorCount = CountTiles(GameObject.FindGameObjectWithTag("Floor").GetComponent<Tilemap>());
            float ketchupTotal = CountTiles(GameObject.FindGameObjectWithTag("Ketchup").GetComponent<Tilemap>());
            var result = (ketchupTotal / floorCount) * 100;
            Debug.Log(result);
            
            spices.SetActive(false);
            ui.SetActive(true);
            endUi.SetActive(true);

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
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ketchup")) _isTouchingKetchup = false;
    }

    void RetryLevel()
    {
        Time.timeScale = 1; // Restaura el tiempo antes de recargar la escena
        SceneManager.LoadScene(currentSceneName); // Recarga la escena actual
    }

    void QuitToMenu()
    {
        Time.timeScale = 1; // Restaura el tiempo antes de cambiar de escena
        SceneManager.LoadScene(menuSceneName); // Carga la escena del men� principal
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
