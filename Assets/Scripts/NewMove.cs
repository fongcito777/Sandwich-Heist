using UnityEngine;

public class NewMove : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    private Rigidbody2D _rb;
    private Vector2 _movement;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _movement = Vector2.zero; // Initialize to zero
    }

    private void Update()
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

    private void FixedUpdate()
    {
        if (_movement != Vector2.zero)
        {
            Vector2 targetPosition = _rb.position + _movement * speed * Time.fixedDeltaTime;
            targetPosition.x = Mathf.Round(targetPosition.x);
            targetPosition.y = Mathf.Round(targetPosition.y);
            _rb.MovePosition(targetPosition);
        }
    }
}