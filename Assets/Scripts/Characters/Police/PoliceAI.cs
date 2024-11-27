/*
 * TODO: Fix stuck on collisions
 */

using System.Collections;
using UnityEngine;

public class PoliceAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] Animator _animator;
    private Vector3 _startingPosition;
    private Rigidbody2D _rb;
    
    [Header("Movement")]
    [SerializeField] private float normalSpeed = 5f;
    [SerializeField] private float slowedSpeed = 2.5f;
    private float _currentSpeed;
    private bool _isSlowed = false;
    
    [Header("Vision Settings")]
    [SerializeField] private float detectionRange = 1f;
    [SerializeField] private float losePlayerRange = 3f;

    [Header("Chase Settings")]
    [SerializeField] private float chaseDelay = 0.5f; //Modify delay for chasing
    private bool _isChaseDelayActive = false;

    private enum PoliceState
    {
        Idle,
        Chasing,
        Returning,
        Noticed
    }
    
    private PoliceState currentState;
    
    private void Start()
    {
        currentState = PoliceState.Idle;
        
        _rb = GetComponent<Rigidbody2D>();
        //_animator = GetComponent<Animator>();
        
        _startingPosition = transform.position;
        _currentSpeed = normalSpeed;
        
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (player == null)
            {
                Debug.LogError("Player not found! Make sure the player has the 'Player' tag.");
            }
        }
        
        if (_animator == null)
        {
            _animator = GetComponent<Animator>();
            if (_animator == null)
            {
                Debug.LogError("Animator not found! Make sure the _animator is attached to the same GameObject.");
            }
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if we entered a slow tile
        if (other.CompareTag("Ketchup"))
        {
            _isSlowed = true;
            _currentSpeed = slowedSpeed;
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        // Check if we exited a slow tile
        if (other.CompareTag("Ketchup"))
        {
            _isSlowed = false;
            _currentSpeed = normalSpeed;
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
                _animator.SetBool("Idle", true);
                _animator.SetBool("Chasing", false);
                _animator.SetBool("Returning", false);
                _animator.SetBool("Noticed", false);
                break;
                
            case PoliceState.Chasing:
                _animator.SetBool("Idle", false);
                _animator.SetBool("Chasing", true);
                _animator.SetBool("Returning", false);
                _animator.SetBool("Noticed", false);
                ChasePlayer();
                break;
                
            case PoliceState.Returning:
                _animator.SetBool("Idle", false);
                _animator.SetBool("Chasing", false);
                _animator.SetBool("Returning", true);
                _animator.SetBool("Noticed", false);
                ReturnToStart();
                break;
            
            case PoliceState.Noticed:
                _animator.SetBool("Idle", false);
                _animator.SetBool("Chasing", false);
                _animator.SetBool("Returning", false);
                _animator.SetBool("Noticed", true);
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
                if (hit.collider.CompareTag("Player"))
                {
                    if (!_isChaseDelayActive)
                    {
                        StartCoroutine(StartChaseAfterDelay());
                    }
                    return _isChaseDelayActive == false;
                }
            }
        }

        return false;
    }

    private IEnumerator StartChaseAfterDelay()
    {
        _isChaseDelayActive = true;

        currentState = PoliceState.Noticed;
        
        yield return new WaitForSeconds(chaseDelay);
        
        currentState = PoliceState.Chasing;

        _isChaseDelayActive = false;
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
            _rb.velocity = Vector2.zero;
            return;
        }
        
        Vector2 moveDirection = GetNonDiagonalDirection(player.position);
        _rb.velocity = moveDirection * _currentSpeed;

        // Pass direction to _animator
        _animator.SetFloat("Horizontal", moveDirection.x);
        _animator.SetFloat("Vertical", moveDirection.y);
    }
    
    private void ReturnToStart()
    {
        float distanceToStart = Vector2.Distance(transform.position, _startingPosition);
        
        if (distanceToStart < 0.1f)
        {
            transform.position = _startingPosition;
            _rb.velocity = Vector2.zero;
            
            currentState = PoliceState.Idle;
            return;
        }
        
        Vector2 moveDirection = GetNonDiagonalDirection(_startingPosition);
        _rb.velocity = moveDirection * _currentSpeed;

        // Pass direction to _animator
        _animator.SetFloat("Horizontal", moveDirection.x);
        _animator.SetFloat("Vertical", moveDirection.y);
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
        if (Application.isPlaying && _rb != null && _rb.velocity != Vector2.zero)
        {
            Gizmos.color = _isSlowed ? Color.cyan : Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)_rb.velocity.normalized);
        }
    }
}
