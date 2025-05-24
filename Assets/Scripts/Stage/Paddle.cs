using UnityEngine;

public class Paddle : MonoBehaviour
{
    public new Rigidbody2D rigidbody { get; private set; }
    public Vector2 direction { get; private set; }
    public float speed = 30f;
    public float maxBounceAngle = 75f;

    private void Awake()
    {
        this.rigidbody = GetComponent<Rigidbody2D>();
    }

    public void ResetPaddle()
    {
        this.transform.position = new Vector2(0f, this.transform.position.y);
        this.rigidbody.linearVelocity = Vector2.zero;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            this.direction = Vector2.left;
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            this.direction = Vector2.right;
        }
        else
        {
            this.direction = Vector2.zero;
        }
    }

    private void FixedUpdate()
    {
        this.rigidbody.linearVelocity = this.direction * this.speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Ball ball = collision.gameObject.GetComponent<Ball>();
        if (ball != null)
        {
            Vector3 paddlePosition = this.transform.position;
            Vector2 contactPoint = collision.GetContact(0).point;

            float offset = contactPoint.x - paddlePosition.x; // ✅ flipped
            float width = collision.otherCollider.bounds.size.x / 2f;

            // Calculate bounce angle
            float bounceAngle = (offset / width) * this.maxBounceAngle;
            float clampedAngle = Mathf.Clamp(bounceAngle, -this.maxBounceAngle, this.maxBounceAngle);

            // Create new direction vector
            float angleInRadians = clampedAngle * Mathf.Deg2Rad;
            Vector2 newDirection = new Vector2(Mathf.Sin(angleInRadians), 1f).normalized;

            // Forcefully apply new consistent velocity
            ball.rigidbody.linearVelocity = newDirection * ball.speed;
        }
    }


}
