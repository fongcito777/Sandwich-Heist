using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewMove : MonoBehaviour
{
    [SerializeField] private float speed;
    private Vector2 _initialDirection;
    private LayerMask _wallMask;
    private Rigidbody2D _rb;
    private Vector2 _direction;
    private Vector2 _nextDirection;
    private Vector3 _startPosition;

    public void ResetState()
    {
        this._direction = this._initialDirection;
        this._nextDirection = Vector2.zero;
        this.transform.position = this._startPosition;
        this._rb.isKinematic = false;
        this.enabled = true;
    }

    private void FixedUpdate()
    {
        var position = this._rb.position;
        var translation = this._direction * this.speed;
        
        this._rb.MovePosition(position + translation);
    }
}
