using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public Transform ray;
    public float groundDistance;
    private Rigidbody2D rb;
    public LayerMask ground;
    public float moveSpeed;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
        if(!isGrounded())
        {
            Flip();
            moveSpeed *= -1;
        }
    }

    private void Flip()
    {
        transform.Rotate(0,180,0);
    }

    private bool isGrounded()
    {
        return Physics2D.Raycast(ray.position, Vector2.down, groundDistance, ground);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(ray.position, new Vector2(ray.position.x, ray.position.y - groundDistance));
    }
}
