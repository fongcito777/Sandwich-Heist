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
    [SerializeField] private float gridSize = 1f;
    private float _currentSpeed;
    private bool _isSlowed = false;
    private bool _isMoving = false;
    private Vector2 _targetPosition;
    
    [Header("Vision Settings")]
    [SerializeField] private float detectionRange = 1f;
    [SerializeField] private float losePlayerRange = 3f;

    [Header("Chase Settings")]
    [SerializeField] private float chaseDelay = 0.5f;
    [SerializeField] private LayerMask wallsLayer;
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
        _startingPosition = transform.position;
        _currentSpeed = normalSpeed;
        _targetPosition = transform.position;
        
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
        if (other.CompareTag("Ketchup"))
        {
            _isSlowed = true;
            _currentSpeed = slowedSpeed;
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ketchup"))
        {
            _isSlowed = false;
            _currentSpeed = normalSpeed;
        }
    }
    
    private void Update()
    {
        if (currentState != PoliceState.Chasing && CheckForPlayer())
        {
            currentState = PoliceState.Chasing;
        }

        UpdateState();
        UpdateAnimation();
    }

    private bool CheckForPlayer()
    {
        if (player == null) return false;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            Vector2 directionToPlayer = (player.position - transform.position).normalized;
            LayerMask layerMask = LayerMask.GetMask("Player", "Walls");
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

    private void UpdateState()
    {
        if (_isMoving) return;

        switch (currentState)
        {
            case PoliceState.Idle:
                _rb.velocity = Vector2.zero;
                break;

            case PoliceState.Chasing:
                if (player == null) return;
                
                float distanceToPlayer = Vector2.Distance(transform.position, player.position);
                if (distanceToPlayer > losePlayerRange)
                {
                    currentState = PoliceState.Returning;
                    return;
                }
                
                TryMove(player.position);
                break;
                
            case PoliceState.Returning:
                float distanceToStart = Vector2.Distance(transform.position, _startingPosition);
                if (distanceToStart < 0.1f)
                {
                    transform.position = _startingPosition;
                    _rb.velocity = Vector2.zero;
                    currentState = PoliceState.Idle;
                    return;
                }
                
                TryMove(_startingPosition);
                break;

            case PoliceState.Noticed:
                _rb.velocity = Vector2.zero;
                break;
        }
    }

    private void UpdateAnimation()
    {
        _animator.SetBool("Idle", currentState == PoliceState.Idle);
        _animator.SetBool("Chasing", currentState == PoliceState.Chasing);
        _animator.SetBool("Returning", currentState == PoliceState.Returning);
        _animator.SetBool("Noticed", currentState == PoliceState.Noticed);
    }

    private Vector2 GetNonDiagonalDirection(Vector2 targetPosition)
    {
        Vector2 direction = Vector2.zero;
        Vector2 difference = targetPosition - (Vector2)transform.position;
        
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

    private void TryMove(Vector2 destination)
    {
        if (_isMoving) return;

        Vector2 direction = GetNonDiagonalDirection(destination);
        Vector2 nextPosition = (Vector2)transform.position + direction * gridSize;
        
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, gridSize, wallsLayer);
        
        if (hit.collider == null)
        {
            StartCoroutine(MoveToNextPosition(nextPosition, direction));
        }
        else
        {
            Vector2 alternateDirection = GetAlternateDirection(destination, direction);
            nextPosition = (Vector2)transform.position + alternateDirection * gridSize;
            
            hit = Physics2D.Raycast(transform.position, alternateDirection, gridSize, wallsLayer);
            if (hit.collider == null)
            {
                StartCoroutine(MoveToNextPosition(nextPosition, alternateDirection));
            }
        }
    }

    private Vector2 GetAlternateDirection(Vector2 destination, Vector2 blockedDirection)
    {
        Vector2 difference = destination - (Vector2)transform.position;
        
        if (blockedDirection.x != 0)
        {
            return new Vector2(0, difference.y > 0 ? 1 : -1);
        }
        else
        {
            return new Vector2(difference.x > 0 ? 1 : -1, 0);
        }
    }

    private IEnumerator MoveToNextPosition(Vector2 nextPos, Vector2 direction)
    {
        _isMoving = true;
        _targetPosition = nextPos;
        _rb.velocity = Vector2.zero;
        
        Vector2 startPos = transform.position;
        float elapsedTime = 0f;
        float moveTime = gridSize / _currentSpeed;

        while (elapsedTime < moveTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / moveTime;
            transform.position = Vector2.Lerp(startPos, nextPos, t);
            
            _animator.SetFloat("Horizontal", direction.x);
            _animator.SetFloat("Vertical", direction.y);
            
            yield return null;
        }

        transform.position = nextPos;
        _isMoving = false;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, losePlayerRange);
        
        if (Application.isPlaying)
        {
            if (_rb != null && _rb.velocity != Vector2.zero)
            {
                Gizmos.color = _isSlowed ? Color.cyan : Color.blue;
                Gizmos.DrawLine(transform.position, transform.position + (Vector3)_rb.velocity.normalized);
            }
            
            Gizmos.color = Color.cyan;
            Vector2 gridPos = new Vector2(
                Mathf.Round(transform.position.x / gridSize) * gridSize,
                Mathf.Round(transform.position.y / gridSize) * gridSize
            );
            Gizmos.DrawWireCube(gridPos, Vector2.one * gridSize);
        }
    }
}
