using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class SpiceManager : MonoBehaviour
{
    // Tilemap
    [SerializeField] private Tilemap targetTilemap;
    [SerializeField] private TileBase tileToPaint;
    private Vector3Int previousTilePosition;
    private BoxCollider2D _bc;
    
    // UI
    public int ketchupCount = 0;
    private TextMeshProUGUI _ketchupText;

    private void Start()
    {
        _bc = GetComponent<BoxCollider2D>();
        _ketchupText = GameObject.Find("KetchupText").GetComponent<TextMeshProUGUI>();
    }
    
    private void Update()
    {
        // UI
        _ketchupText.text = "Ketchup: " + ketchupCount.ToString();
        
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
    }
}
