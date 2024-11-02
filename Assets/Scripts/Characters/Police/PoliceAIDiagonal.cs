using UnityEngine;

public class PoliceAIDiagonal : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    private Vector3 startingPosition;
    private Rigidbody2D rb;
    
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    
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
        
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (player == null)
            {
                Debug.LogError("Player not found! Make sure the player has the 'Player' tag.");
            }
        }
    }
    
    private void Update()
    {
        switch (currentState)
        {
            case PoliceState.Idle:
                CheckForPlayer();
                break;
                
            case PoliceState.Chasing:
                ChasePlayer();
                break;
                
            case PoliceState.Returning:
                ReturnToStart();
                break;
        }
    }
    
    private void CheckForPlayer()
    {
        if (player == null) return;
        
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        // If player is within detection range, start chasing
        if (distanceToPlayer <= detectionRange)
        {
            currentState = PoliceState.Chasing;
        }
    }
    
    private void ChasePlayer()
    {
        if (player == null) return;
        
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        // If player is too far, stop chasing and return
        if (distanceToPlayer > losePlayerRange)
        {
            currentState = PoliceState.Returning;
            return;
        }
        
        // Move towards player
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * moveSpeed;
    }
    
    private void ReturnToStart()
    {
        // Calculate distance to starting position
        float distanceToStart = Vector2.Distance(transform.position, startingPosition);
        
        // If we're very close to the starting position, stop moving
        if (distanceToStart < 0.1f)
        {
            transform.position = startingPosition;
            rb.velocity = Vector2.zero;
            currentState = PoliceState.Idle;
            return;
        }
        
        // Move towards starting position
        Vector2 direction = (startingPosition - transform.position).normalized;
        rb.velocity = direction * moveSpeed;
    }
    
    // Optional: Visualize the detection ranges in the editor
    private void OnDrawGizmos()
    {
        // Detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Lose player range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, losePlayerRange);
    }
}
