using UnityEngine;

public class Ball : MonoBehaviour
{
    public new Rigidbody2D rigidbody { get; private set; }
    public float speed = 500f;
    public float distanceFromPaddle = 1f;

    public AudioClip wallHitSFX;
    public AudioClip brickHitSFX;
    public AudioClip paddleHitSFX;
    private AudioSource audioSource;

    private bool isLaunched = false;
    private Transform paddle; 

    private void Awake()
    {
        this.rigidbody = GetComponent<Rigidbody2D>();
        paddle = GameObject.FindGameObjectWithTag("Paddle").transform;  
    }

    private void Start()
    {
        ResetBall();
        audioSource = GetComponent<AudioSource>();
    }

    public void ResetBall()
    {
        this.transform.position = paddle.position + new Vector3(0, distanceFromPaddle, 0);
        this.rigidbody.linearVelocity = Vector2.zero;
        isLaunched = false;
    }
    public void Launch()
    {
        if (!isLaunched)
        {
            SetRandomTrajectory();
            isLaunched = true; 
        }
    }

    private void SetRandomTrajectory()
    {
        Vector2 force = new Vector2(Random.Range(-1f, 1f), -1f);
        force.Normalize(); 
        this.rigidbody.AddForce(force * speed);
    }

    private void Update()
    {
        if (!isLaunched) 
        {
            transform.position = paddle.position + new Vector3(0, distanceFromPaddle, 0); 
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Launch();
            }
        }
    }

    private void FixedUpdate()
    {
        if (rigidbody.linearVelocity.magnitude > speed)
        {
            rigidbody.linearVelocity = rigidbody.linearVelocity.normalized * speed;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            audioSource.PlayOneShot(wallHitSFX);
        }
        else if (collision.gameObject.CompareTag("Brick"))
        {
            audioSource.PlayOneShot(brickHitSFX);
        }
        else if (collision.gameObject.CompareTag("Paddle"))
        {
            audioSource.PlayOneShot(paddleHitSFX);
            rigidbody.linearVelocity = rigidbody.linearVelocity.normalized * speed;
        }
    }
}
