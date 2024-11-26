using System;
using UnityEngine;

public class Move : MonoBehaviour
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

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _transform = GetComponent<Transform>();
        _animator = GetComponent<Animator>();
        _targetPosition = _rb.position;

        // Ensure starting position is on the offset grid
        Vector2 startPos = _rb.position;
        _rb.position = new Vector2(
            Mathf.Floor(startPos.x) + GRID_OFFSET,
            Mathf.Floor(startPos.y) + GRID_OFFSET
        );
    }

    private void Update()
    {
        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");

        // Check for new input and change direction immediately
        if (horizontal != 0 || vertical != 0)
        {
            Vector2 newDirection = Vector2.zero;

            // Prioritize the latest input
            if (horizontal != 0)
            {
                newDirection = new Vector2(horizontal, 0);

                // Handle character flipping
                if ((horizontal > 0 && !_facingRight) || (horizontal < 0 && _facingRight))
                {
                    Flip();
                }
            }
            else if (vertical != 0)
            {
                newDirection = new Vector2(0, vertical);
            }

            // If direction changed, update target position
            if (newDirection != _lastDirection)
            {
                _lastDirection = newDirection;
                _targetPosition = CalculateNextGridPosition(newDirection);
                _isMoving = true;
            }
        }

        // Update animator parameters
        _animator.SetFloat("Horizontal", Mathf.Abs(_lastDirection.x));
        _animator.SetFloat("Vertical", _lastDirection.y);
        _animator.SetFloat("Speed", _isMoving ? speed : 0);

        // Check if we've reached the target position
        if (_isMoving && Vector2.Distance(_rb.position, _targetPosition) < 0.01f)
        {
            _rb.position = _targetPosition;

            // Set next target position if we're still moving in the same direction
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

    private Vector2 CalculateNextGridPosition(Vector2 direction)
    {
        Vector2 currentPos = _rb.position;

        // Calculate the next grid position, accounting for the 0.5 offset
        float nextX = Mathf.Floor(currentPos.x) + GRID_OFFSET;
        float nextY = Mathf.Floor(currentPos.y) + GRID_OFFSET;

        // If we're already close to a grid position, move to the next one
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
        Debug.Log("Collision Enter");
        if (other.gameObject.CompareTag("Walls"))
        {
            Debug.Log("Wall");
            _isMoving = false;
        }
    }
}
