using System;
using UnityEngine;

public class Move : MonoBehaviour
{
    // Movement
    [SerializeField] private float speed = 5f;
    private Rigidbody2D _rb;
    private Vector2 _movement;
    private Transform _transform;
    private bool _moved;
    private Transform _movePoint;
    
    // Animations
    private Animator _animator;
    private bool _facingRight = true;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _transform = GetComponent<Transform>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!Input.anyKeyDown) return;
        _moved = true;
            
        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");
            
        _animator.SetFloat("Horizontal", Mathf.Abs(horizontal));
        _animator.SetFloat("Vertical", vertical);
        _animator.SetFloat("Speed", speed);

        if (!_moved || (!(Mathf.Abs(transform.position.x % 1) > 0.01f) &&
                        !(Mathf.Abs(transform.position.y % 1) > 0.01f))) return;
        
        if (horizontal != 0)
        {
            _movement.x = horizontal;
            _movement.y = 0;

            switch (horizontal)
            {
                // Flip the character based on the direction
                case > 0 when !_facingRight:
                case < 0 when _facingRight:
                    Flip();
                    break;
            }
        }
        else if (vertical != 0)
        {
            _movement.x = 0;
            _movement.y = vertical;
        }
        _moved = false;
    }

    private void FixedUpdate()
    {
        _rb.MovePosition(_rb.position + _movement * (speed * Time.fixedDeltaTime));
    }

    private void Flip()
    {
        _facingRight = !_facingRight;
        var scale = _transform.localScale;
        scale.x *= -1;
        _transform.localScale = scale;
    }

    private void OnTriggerEnter(Collider other)
    {
        throw new NotImplementedException();
    }
}
