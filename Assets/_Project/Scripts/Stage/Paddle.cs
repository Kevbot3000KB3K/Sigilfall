using UnityEngine;

/// <summary>
/// Handles player paddle movement, input, and ball bounce physics.
/// </summary>
public class Paddle : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 30f;

    [Header("Ball Bounce Settings")]
    public float maxBounceAngle = 75f;

    public new Rigidbody2D rigidbody { get; private set; }
    public Vector2 direction { get; private set; }

    private void Awake()
    {
        // Cache the Rigidbody2D component
        rigidbody = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Resets the paddle's position and velocity.
    /// </summary>
    public void ResetPaddle()
    {
        transform.position = new Vector2(0f, transform.position.y);
        rigidbody.linearVelocity = Vector2.zero;
    }

    /// <summary>
    /// Captures horizontal movement input each frame.
    /// </summary>
    private void Update()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            direction = Vector2.left;
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            direction = Vector2.right;
        }
        else
        {
            direction = Vector2.zero;
        }
    }

    /// <summary>
    /// Applies movement based on player input using physics.
    /// </summary>
    private void FixedUpdate()
    {
        rigidbody.linearVelocity = direction * speed;
    }

    /// <summary>
    /// Handles bounce logic when the ball hits the paddle.
    /// Adjusts the ball's angle based on contact point.
    /// </summary>
    /// <param name="collision">Collision with the ball.</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Ball ball = collision.gameObject.GetComponent<Ball>();
        if (ball == null) return;

        Vector3 paddlePos = transform.position;
        Vector2 contactPoint = collision.GetContact(0).point;

        // Calculate horizontal offset from paddle center
        float offset = contactPoint.x - paddlePos.x;
        float width = collision.otherCollider.bounds.size.x / 2f;

        // Convert to bounce angle
        float bounceAngle = (offset / width) * maxBounceAngle;
        bounceAngle = Mathf.Clamp(bounceAngle, -maxBounceAngle, maxBounceAngle);

        float angleRad = bounceAngle * Mathf.Deg2Rad;
        Vector2 newDirection = new Vector2(Mathf.Sin(angleRad), 1f).normalized;

        // Apply new velocity to the ball
        ball.rigidbody.linearVelocity = newDirection * ball.speed;
    }
}
