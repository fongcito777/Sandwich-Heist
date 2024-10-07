using UnityEngine;

public class Move : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    private Rigidbody2D _rb;
    private Vector2 _movement;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!Input.anyKeyDown) {}
        else
        {
            var horizontal = Input.GetAxisRaw("Horizontal");
            var vertical = Input.GetAxisRaw("Vertical");

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
        }
    }

    private void FixedUpdate()
    {
        _rb.MovePosition(_rb.position + _movement * (speed * Time.fixedDeltaTime));
    }
}
