using UnityEngine;

public class Move : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float gridSize = 1f;
    [SerializeField] private float inputBufferTime = 0.05f;
    [SerializeField] private float minSwipeDistance = 30f;
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

    // Touch input variables
    private Vector2 _startTouchPosition;
    private Vector2 _endTouchPosition;
    private bool _swipeDetected;

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
        HandleTouchInput();
        HandleKeyboardInput();
        
        ProcessMovement();
        UpdateAnimator();
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                _startTouchPosition = touch.position;
                _swipeDetected = false;
            }
            else if (touch.phase == TouchPhase.Ended && !_swipeDetected)
            {
                _endTouchPosition = touch.position;
                Vector2 swipeDirection = _endTouchPosition - _startTouchPosition;

                if (swipeDirection.magnitude > minSwipeDistance)
                {
                    swipeDirection.Normalize();
                    ProcessSwipe(swipeDirection);
                    _swipeDetected = true;
                }
            }
        }
    }

    private void HandleKeyboardInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        UpdateInputBuffer(horizontal, vertical);
    }

    private void ProcessSwipe(Vector2 swipeDirection)
    {
        float horizontal = 0f;
        float vertical = 0f;

        if (Mathf.Abs(swipeDirection.x) > Mathf.Abs(swipeDirection.y))
        {
            horizontal = swipeDirection.x > 0 ? 1f : -1f;
        }
        else
        {
            vertical = swipeDirection.y > 0 ? 1f : -1f;
        }

        UpdateInputBuffer(horizontal, vertical);
    }

    private void UpdateInputBuffer(float horizontal, float vertical)
    {
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

        if ((_bufferedHorizontal != 0 || _bufferedVertical != 0) && canChangeDirection)
        {
            Vector2 newDirection = Vector2.zero;

            if (_bufferedHorizontal != 0)
            {
                newDirection = new Vector2(_bufferedHorizontal, 0);
                if (!CanMoveInDirection(newDirection) && _bufferedVertical != 0)
                {
                    newDirection = new Vector2(0, _bufferedVertical);
                }
            }
            else if (_bufferedVertical != 0)
            {
                newDirection = new Vector2(0, _bufferedVertical);
            }

            if (newDirection != Vector2.zero && CanMoveInDirection(newDirection))
            {
                _lastDirection = newDirection;
                _targetPosition = CalculateNextGridPosition(newDirection);
                _isMoving = true;

                Vector2 currentGridPos = GetNearestGridPosition(_rb.position);
                if (Vector2.Distance(_rb.position, currentGridPos) < POSITION_TOLERANCE)
                {
                    _rb.position = currentGridPos;
                }

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

        if (_isMoving && Vector2.Distance(_rb.position, _targetPosition) < POSITION_TOLERANCE)
        {
            _rb.position = _targetPosition;

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
