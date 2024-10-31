using UnityEngine;

public class Move : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    private Rigidbody2D _rb;
    private Vector2 _movement;
    private bool moved = false;
    private Transform _transform;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _transform = GetComponent<Transform>();
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            moved = true;
            var horizontal = Input.GetAxisRaw("Horizontal");
            var vertical = Input.GetAxisRaw("Vertical");
            
            if (moved && (transform.position.x % 1 == 0 || transform.position.y % 1 == 0))
            {
                if (horizontal != 0)
                {
                    _movement.x = horizontal;
                    _movement.y = 0;
                }
                else if (vertical != 0)
                {
                    _movement.x = 0;
                    _movement.y = vertical;
                }
                moved = false;
            }

        }
    }

    private void FixedUpdate()
    {
        _rb.MovePosition(_rb.position + _movement * (speed * Time.fixedDeltaTime));
    }
}
