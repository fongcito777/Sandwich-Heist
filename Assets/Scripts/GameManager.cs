using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
     private GameObject _player;
     private Tilemap _floorTm;
     private Tilemap _ketchupTm;
     private Camera _mainCamera;
     
     private void Start()
     {
         _player = GameObject.FindGameObjectWithTag("Player");
         _floorTm = GameObject.FindGameObjectWithTag("Floor").GetComponent<Tilemap>();
         _mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
     }
     
     void Update()
     {
     }
 }