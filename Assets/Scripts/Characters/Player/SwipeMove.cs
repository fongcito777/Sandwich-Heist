using UnityEngine;

public class SwipeMove : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float gridSize = 1f;
    private const float GRID_OFFSET = 0.5f;

    private Rigidbody2D _rb;
    private Transform _transform;
    private Animator _animator;
    private bool _facingRight = true;

    private Vector2 _targetPosition;
    private Vector2 _lastDirection;
    private bool _isMoving;

    private Vector2 _startTouchPosition;
    private Vector2 _endTouchPosition;
    private bool _swipeDetected;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _transform = GetComponent<Transform>();
        _animator = GetComponent<Animator>();
        _targetPosition = _rb.position;

        Vector2 startPos = _rb.position;
        _rb.position = new Vector2(
            Mathf.Floor(startPos.x) + GRID_OFFSET,
            Mathf.Floor(startPos.y) + GRID_OFFSET
        );
    }

    private void Update()
    {
        HandleTouchInput();

        // Update animator parameters
        _animator.SetFloat("Horizontal", Mathf.Abs(_lastDirection.x));
        _animator.SetFloat("Vertical", _lastDirection.y);
        _animator.SetFloat("Speed", _isMoving ? speed : 0);

        // Check if we've reached the target position
        if (_isMoving && Vector2.Distance(_rb.position, _targetPosition) < 0.01f)
        {
            _rb.position = _targetPosition;

            if (_lastDirection != Vector2.zero)
            {
                _targetPosition = CalculateNextGridPosition(_lastDirection);
            }
            else
            {
                _isMoving = false;
            }
        }
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

                if (swipeDirection.magnitude > 50) // Threshold for swipe detection
                {
                    swipeDirection.Normalize();
                    DetermineSwipeDirection(swipeDirection);
                    _swipeDetected = true;
                }
            }
        }
    }

    private void DetermineSwipeDirection(Vector2 swipeDirection)
    {
        Vector2 newDirection = Vector2.zero;

        if (Mathf.Abs(swipeDirection.x) > Mathf.Abs(swipeDirection.y))
        {
            // Horizontal swipe
            newDirection = swipeDirection.x > 0 ? Vector2.right : Vector2.left;

            if ((swipeDirection.x > 0 && !_facingRight) || (swipeDirection.x < 0 && _facingRight))
            {
                Flip();
            }
        }
        else
        {
            // Vertical swipe
            newDirection = swipeDirection.y > 0 ? Vector2.up : Vector2.down;
        }

        if (newDirection != _lastDirection)
        {
            _lastDirection = newDirection;
            _targetPosition = CalculateNextGridPosition(newDirection);
            _isMoving = true;
        }
    }

    private Vector2 CalculateNextGridPosition(Vector2 direction)
    {
        Vector2 currentPos = _rb.position;

        float nextX = Mathf.Floor(currentPos.x) + GRID_OFFSET;
        float nextY = Mathf.Floor(currentPos.y) + GRID_OFFSET;

        if (Mathf.Abs(currentPos.x - nextX) < 0.01f)
        {
            nextX += direction.x * gridSize;
        }
        if (Mathf.Abs(currentPos.y - nextY) < 0.01f)
        {
            nextY += direction.y * gridSize;
        }

        return new Vector2(nextX, nextY);
    }

    private void FixedUpdate()
    {
        if (_isMoving)
        {
            Vector2 newPosition = Vector2.MoveTowards(_rb.position, _targetPosition,
                                                speed * Time.fixedDeltaTime);
            _rb.MovePosition(newPosition);
        }
    }

    private void Flip()
    {
        _facingRight = !_facingRight;
        var scale = _transform.localScale;
        scale.x *= -1;
        _transform.localScale = scale;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Walls"))
        {
            _isMoving = false;
        }
    }
}
