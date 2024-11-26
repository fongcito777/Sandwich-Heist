using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PoliceAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    private Vector3 startingPosition;
    private Rigidbody2D rb;
    
    [Header("Movement")]
    [SerializeField] private float normalSpeed = 5f;
    [SerializeField] private float slowedSpeed = 2.5f;
    private float currentSpeed;
    private bool isSlowed = false;
    
    [Header("Vision Settings")]
    [SerializeField] private float detectionRange = 1f;
    [SerializeField] private float losePlayerRange = 3f;
    
    private enum PoliceState
    {
        Idle,
        Chasing,
        Returning
    }
    
    private PoliceState currentState = PoliceState.Idle;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startingPosition = transform.position;
        currentSpeed = normalSpeed;
        
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (player == null)
            {
                Debug.LogError("Player not found! Make sure the player has the 'Player' tag.");
            }
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if we entered a slow tile
        if (other.CompareTag("Ketchup"))
        {
            isSlowed = true;
            currentSpeed = slowedSpeed;
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        // Check if we exited a slow tile
        if (other.CompareTag("Ketchup"))
        {
            isSlowed = false;
            currentSpeed = normalSpeed;
        }
    }
    
    private void Update()
    {
        // Always check for player first, regardless of current state
        if (currentState != PoliceState.Chasing && CheckForPlayer())
        {
            currentState = PoliceState.Chasing;
        }
        
        // Then handle state-specific behavior
        switch (currentState)
        {
            case PoliceState.Idle:
                break;
                
            case PoliceState.Chasing:
                ChasePlayer();
                break;
                
            case PoliceState.Returning:
                ReturnToStart();
                break;
        }
    }

    private bool CheckForPlayer()
    {
        if (player == null) return false;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            Vector2 directionToPlayer = (player.position - transform.position).normalized;

            // Configurar el LayerMask para ignorar capas innecesarias
            LayerMask layerMask = LayerMask.GetMask("Player", "Walls");

            // Lanzar el Raycast
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, detectionRange, layerMask);

            if (hit.collider != null)
            {
                Debug.Log("Collider Tag: " + hit.collider.tag);

                if (hit.collider.CompareTag("Player"))
                {
                    return true;
                }
            }
        }

        return false;
    }


    private Vector2 GetNonDiagonalDirection(Vector2 targetPosition)
    {
        Vector2 direction = Vector2.zero;
        Vector2 difference = targetPosition - (Vector2)transform.position;
        
        // Move in the direction of the largest difference (horizontal or vertical)
        if (Mathf.Abs(difference.x) > Mathf.Abs(difference.y))
        {
            direction.x = difference.x > 0 ? 1 : -1;
        }
        else
        {
            direction.y = difference.y > 0 ? 1 : -1;
        }
        
        return direction;
    }
    
    private void ChasePlayer()
    {
        if (player == null) return;
        
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        if (distanceToPlayer > losePlayerRange)
        {
            currentState = PoliceState.Returning;
            rb.velocity = Vector2.zero;
            return;
        }
        
        Vector2 moveDirection = GetNonDiagonalDirection(player.position);
        rb.velocity = moveDirection * currentSpeed;
    }
    
    private void ReturnToStart()
    {
        float distanceToStart = Vector2.Distance(transform.position, startingPosition);
        
        if (distanceToStart < 0.1f)
        {
            transform.position = startingPosition;
            rb.velocity = Vector2.zero;
            currentState = PoliceState.Idle;
            return;
        }
        
        Vector2 moveDirection = GetNonDiagonalDirection(startingPosition);
        rb.velocity = moveDirection * currentSpeed;
    }
    
    // Dev: Visual representation of movement direction and speed
    private void OnDrawGizmos()
    {
        // Detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Lose player range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, losePlayerRange);
        
        // Current movement direction
        if (Application.isPlaying && rb != null && rb.velocity != Vector2.zero)
        {
            Gizmos.color = isSlowed ? Color.cyan : Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)rb.velocity.normalized);
        }
    }
}
