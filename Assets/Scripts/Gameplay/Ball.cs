using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class Ball : MonoBehaviour
{
    public new Rigidbody2D rigidbody { get; private set; }
    public float speed = 500f;
    public float distanceFromPaddle = 1f;
    public float damage = 1f; // Default damage for a regular ball hit
    private bool isStuckToPaddle = false;

    public List<BallEffect> activeEffects;


    public AudioClip wallHitSFX;
    public AudioClip brickHitSFX;
    public AudioClip paddleHitSFX;
    private AudioSource audioSource;
    private bool homingActive = false;
    private bool isLaunched = false;
    private Transform paddle;
    private readonly List<IEnumerator> temporaryEffectCoroutines = new();

    private bool magnetActive = false;
    public bool hasKillzoneReflectPowerup = false;
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
    public void StickToPaddle()
    {
        isStuckToPaddle = true;
        rigidbody.linearVelocity = Vector2.zero;
    }
    public void ApplyEffectTemporary(System.Action<Ball> effectStart, System.Action<Ball> effectEnd, float duration)
    {
        IEnumerator Routine()
        {
            effectStart(this);
            yield return new WaitForSeconds(duration);
            effectEnd(this);
        }

        StartCoroutine(Routine());
    }
    private void SetRandomTrajectory()
    {
        float x = Random.Range(-1f, 1f);
        float y = Random.Range(-0.5f, -1f);

        Vector2 direction = new Vector2(x, y).normalized;
        this.rigidbody.AddForce(direction * speed);
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

        // Update active effects
        foreach (var effect in activeEffects)
        {
            effect.OnUpdate(this);
        }
        if (magnetActive)
        {
            Vector2 targetX = new Vector2(paddle.position.x, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, targetX, Time.deltaTime * 2f); // Smooth pull toward paddle x
        }
        if (homingActive)
        {
            Brick[] bricks = FindObjectsOfType<Brick>();
            if (bricks.Length > 0)
            {
                // Find closest brick
                Brick closest = null;
                float minDist = float.MaxValue;
                foreach (var b in bricks)
                {
                    float dist = Vector2.Distance(transform.position, b.transform.position);
                    if (b.gameObject.activeInHierarchy && dist < minDist)
                    {
                        closest = b;
                        minDist = dist;
                    }
                }

                if (closest != null)
                {
                    Vector2 dir = (closest.transform.position - transform.position).normalized;
                    rigidbody.linearVelocity = Vector2.Lerp(rigidbody.linearVelocity, dir * speed, Time.deltaTime * 2f);
                }
            }
        }

    }

    public void OnHitKillzone()
    {
        if (hasKillzoneReflectPowerup)
        {
            hasKillzoneReflectPowerup = false;
            // Bounce the ball off the killzone instead of losing life or destroying
            Vector2 velocity = rigidbody.linearVelocity;
            rigidbody.linearVelocity = new Vector2(velocity.x, -velocity.y); // simple vertical bounce
            Debug.Log("Killzone reflect triggered!");
        }
        else
        {
            // Normal killzone behavior (lose life, reset ball, etc)
        }
    }

    public void StartMagnetEffect()
    {
        magnetActive = true;
        Debug.Log("Magnet effect started!");
    }
    public void StopMagnetEffect()
    {
        magnetActive = false;
        Debug.Log("Magnet effect ended!");
    }
    private void FixedUpdate()
    {
        if (rigidbody.linearVelocity.magnitude > speed)
        {
            rigidbody.linearVelocity = rigidbody.linearVelocity.normalized * speed;
        }
    }
    public void StartHomingEffect()
    {
        homingActive = true;
        Debug.Log("Homing effect started!");
    }
    public void StopHomingEffect()
    {
        homingActive = false;
        Debug.Log("Homing effect ended!");
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

            Brick brick = collision.gameObject.GetComponent<Brick>();
            foreach (var effect in activeEffects)
            {
                effect.OnHitBrick(this, brick);
            }
        }
        else if (collision.gameObject.CompareTag("Paddle"))
        {
            audioSource.PlayOneShot(paddleHitSFX);
            rigidbody.linearVelocity = rigidbody.linearVelocity.normalized * speed;

            Paddle paddleScript = collision.gameObject.GetComponent<Paddle>();
            foreach (var effect in activeEffects)
            {
                effect.OnHitPaddle(this, paddleScript);
            }
        }
    }

}
