using UnityEngine;

public class Move : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float gridSize = 1f;
    [SerializeField] private float inputBufferTime = 0.1f; // How long to remember the input
    private const float GRID_OFFSET = 0.5f;
    private const float POSITION_TOLERANCE = 0.01f;

    private Rigidbody2D _rb;
    private Transform _transform;
    private Animator _animator;
    private BoxCollider2D _collider;

    private Vector2 _targetPosition;
    private Vector2 _lastDirection;
    private bool _isMoving;
    private ContactFilter2D _contactFilter;
    private RaycastHit2D[] _raycastHits = new RaycastHit2D[1];

    // Input buffer variables
    private float _horizontalBufferTimeLeft;
    private float _verticalBufferTimeLeft;
    private float _bufferedHorizontal;
    private float _bufferedVertical;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _transform = GetComponent<Transform>();
        _animator = GetComponent<Animator>();
        _collider = GetComponent<BoxCollider2D>();
        _targetPosition = _rb.position;

        _contactFilter = new ContactFilter2D();
        _contactFilter.SetLayerMask(LayerMask.GetMask("Walls"));
        _contactFilter.useLayerMask = true;

        SnapToGrid();
    }

    private void Update()
    {
        UpdateInputBuffer();
        ProcessMovement();
        UpdateAnimator();
    }

    private void UpdateInputBuffer()
    {
        // Get current input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Update horizontal buffer
        if (horizontal != 0)
        {
            _bufferedHorizontal = horizontal;
            _horizontalBufferTimeLeft = inputBufferTime;
        }
        else if (_horizontalBufferTimeLeft > 0)
        {
            _horizontalBufferTimeLeft -= Time.deltaTime;
            if (_horizontalBufferTimeLeft <= 0)
            {
                _bufferedHorizontal = 0;
            }
        }

        // Update vertical buffer
        if (vertical != 0)
        {
            _bufferedVertical = vertical;
            _verticalBufferTimeLeft = inputBufferTime;
        }
        else if (_verticalBufferTimeLeft > 0)
        {
            _verticalBufferTimeLeft -= Time.deltaTime;
            if (_verticalBufferTimeLeft <= 0)
            {
                _bufferedVertical = 0;
            }
        }
    }

    private void ProcessMovement()
    {
        bool canChangeDirection = Vector2.Distance(_rb.position, GetNearestGridPosition(_rb.position)) < POSITION_TOLERANCE;

        // Check if we can change direction and have buffered input
        if ((_bufferedHorizontal != 0 || _bufferedVertical != 0) && canChangeDirection)
        {
            Vector2 newDirection = Vector2.zero;

            // Try horizontal movement first
            if (_bufferedHorizontal != 0)
            {
                newDirection = new Vector2(_bufferedHorizontal, 0);
                if (!CanMoveInDirection(newDirection) && _bufferedVertical != 0)
                {
                    // If horizontal movement is blocked and we have vertical input, try that instead
                    newDirection = new Vector2(0, _bufferedVertical);
                }
            }
            else if (_bufferedVertical != 0)
            {
                newDirection = new Vector2(0, _bufferedVertical);
            }

            // Update movement if we can move in the new direction
            if (newDirection != Vector2.zero && CanMoveInDirection(newDirection))
            {
                _lastDirection = newDirection;
                _targetPosition = CalculateNextGridPosition(newDirection);
                _isMoving = true;

                // Snap to grid when changing direction
                Vector2 currentGridPos = GetNearestGridPosition(_rb.position);
                if (Vector2.Distance(_rb.position, currentGridPos) < POSITION_TOLERANCE)
                {
                    _rb.position = currentGridPos;
                }

                // Clear the used input buffer
                if (newDirection.x != 0)
                {
                    _horizontalBufferTimeLeft = 0;
                    _bufferedHorizontal = 0;
                }
                else if (newDirection.y != 0)
                {
                    _verticalBufferTimeLeft = 0;
                    _bufferedVertical = 0;
                }
            }
        }

        // Check if reached target position
        if (_isMoving && Vector2.Distance(_rb.position, _targetPosition) < POSITION_TOLERANCE)
        {
            _rb.position = _targetPosition;

            // Set next target position if we can keep moving in the same direction
            if (_lastDirection != Vector2.zero && CanMoveInDirection(_lastDirection))
            {
                _targetPosition = CalculateNextGridPosition(_lastDirection);
            }
            else
            {
                _isMoving = false;
            }
        }
    }

    private void SnapToGrid()
    {
        Vector2 startPos = _rb.position;
        _rb.position = new Vector2(
            Mathf.Floor(startPos.x) + GRID_OFFSET,
            Mathf.Floor(startPos.y) + GRID_OFFSET
        );
        _targetPosition = _rb.position;
    }

    private bool CanMoveInDirection(Vector2 direction)
    {
        Vector2 rayStart = _rb.position;
        float rayLength = gridSize * 0.8f;
        int hitCount = Physics2D.Raycast(rayStart, direction, _contactFilter, _raycastHits, rayLength);
        return hitCount == 0;
    }

    private Vector2 GetNearestGridPosition(Vector2 position)
    {
        return new Vector2(
            Mathf.Floor(position.x) + GRID_OFFSET,
            Mathf.Floor(position.y) + GRID_OFFSET
        );
    }

    private Vector2 CalculateNextGridPosition(Vector2 direction)
    {
        Vector2 currentGridPos = GetNearestGridPosition(_rb.position);
        return currentGridPos + (direction * gridSize);
    }

    private void UpdateAnimator()
    {
        _animator.SetFloat("Horizontal", _lastDirection.x);
        _animator.SetFloat("Vertical", _lastDirection.y);
        _animator.SetFloat("Speed", _isMoving ? speed : 0);
    }

    private void FixedUpdate()
    {
        if (_isMoving)
        {
            Vector2 newPosition = Vector2.MoveTowards(
                _rb.position,
                _targetPosition,
                speed * Time.fixedDeltaTime
            );
            _rb.MovePosition(newPosition);
        }
    }
}
