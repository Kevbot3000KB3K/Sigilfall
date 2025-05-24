using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Handles the physics, behavior, and special effects of the ball in the brick breaker game.
/// Includes logic for launching, sticking to paddle, applying effects, and collisions.
/// </summary>
public class Ball : MonoBehaviour
{
    public new Rigidbody2D rigidbody { get; private set; }

    [Header("Ball Properties")]
    public float speed = 500f;
    public float distanceFromPaddle = 1f;
    public float damage = 1f;
    public bool skipInitialReset = false;
    public bool isLaunched { get; set; }
    public bool attachedToPaddle = true;

    [Header("Audio Clips")]
    public AudioClip wallHitSFX;
    public AudioClip brickHitSFX;
    public AudioClip paddleHitSFX;
    public AudioClip launchSFX;

    [Header("Effects")]
    public List<BallEffect> activeEffects;

    private AudioSource audioSource;
    private Transform paddle;
    private bool magnetActive = false;
    private bool homingActive = false;
    private readonly List<IEnumerator> temporaryEffectCoroutines = new();
    public bool hasKillzoneReflectPowerup = false;
    public bool isGrown = false;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        paddle = GameObject.FindGameObjectWithTag("Paddle").transform;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (!skipInitialReset)
            ResetBall();

        GameManager.Instance.RegisterBall();

        if (PlayerData.Instance != null && PlayerData.Instance.HasEquippedSigil())
        {
            Sigil mySigil = PlayerData.Instance.equippedSigil;

            // Sprite & trail
            GetComponent<SpriteRenderer>().sprite = mySigil.sigilSprite;
            var trail = GetComponent<TrailRenderer>();
            trail.startColor = trail.endColor = mySigil.trailColor;

            if (mySigil.special != null)
            {
                activeEffects ??= new List<BallEffect>();
                activeEffects.Add(mySigil.special);
            }
        }

        // Remove any null effects
        activeEffects = activeEffects?.Where(e => e != null).ToList();
    }

    /// <summary>
    /// Resets the ball's position and state to follow the paddle before launch.
    /// </summary>
    public void ResetBall()
    {
        transform.position = paddle.position + Vector3.up * distanceFromPaddle;
        rigidbody.linearVelocity = Vector2.zero;
        isLaunched = false;
        attachedToPaddle = true;

        var trail = GetComponent<TrailRenderer>();
        if (trail != null)
        {
            trail.Clear();
            trail.enabled = false;
        }
    }

    /// <summary>
    /// Launches the ball in a random upward direction.
    /// </summary>
    public void Launch()
    {
        if (isLaunched) return;

        isLaunched = true;
        attachedToPaddle = false;
        SetRandomTrajectory();

        var trail = GetComponent<TrailRenderer>();
        if (trail != null)
        {
            trail.Clear();
            trail.enabled = true;
        }

        if (launchSFX != null)
            audioSource?.PlayOneShot(launchSFX);
    }

    /// <summary>
    /// Snaps the ball to the paddle and disables its movement.
    /// </summary>
    public void StickToPaddle()
    {
        attachedToPaddle = true;
        isLaunched = false;
        rigidbody.linearVelocity = Vector2.zero;

        var trail = GetComponent<TrailRenderer>();
        if (trail != null)
            trail.enabled = false;

        Debug.Log("Ball has stuck to paddle!");
    }

    /// <summary>
    /// Applies a temporary effect to the ball for a duration.
    /// </summary>
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

    /// <summary>
    /// Applies a random upward velocity to launch the ball.
    /// </summary>
    private void SetRandomTrajectory()
    {
        float x = Random.Range(-1f, 1f);
        float y = Random.Range(0.5f, 1f);
        Vector2 direction = new Vector2(x, y).normalized;
        rigidbody.linearVelocity = direction * speed;
    }

    private void Update()
    {
        // Follow paddle until launched
        if (!isLaunched && attachedToPaddle)
        {
            transform.position = paddle.position + Vector3.up * distanceFromPaddle;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Launch();
            }
        }

        // Active effect updates
        if (activeEffects != null)
        {
            foreach (var effect in activeEffects)
                effect.OnUpdate(this);
        }

        // Magnet effect: pulls ball horizontally toward paddle
        if (magnetActive && paddle != null)
        {
            Vector2 ballPos = transform.position;
            Vector2 target = new Vector2(paddle.position.x, ballPos.y);
            transform.position = Vector2.Lerp(ballPos, target, Time.deltaTime * 2.5f);
        }

        // Homing effect: gradually targets nearest brick
        if (homingActive)
        {
            Brick[] bricks = Object.FindObjectsByType<Brick>(FindObjectsSortMode.None);
            if (bricks.Length > 0)
            {
                Brick closest = null;
                float minDist = float.MaxValue;
                foreach (var b in bricks)
                {
                    if (!b.gameObject.activeInHierarchy) continue;
                    float dist = Vector2.Distance(transform.position, b.transform.position);
                    if (dist < minDist)
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

    private void FixedUpdate()
    {
        // Prevent slow/stuck ball
        if (isLaunched && rigidbody.linearVelocity.magnitude < 0.01f)
        {
            Debug.LogWarning("Ball velocity too low — reapplying upward nudge.");
            rigidbody.linearVelocity = new Vector2(Random.Range(-0.5f, 0.5f), 1f).normalized * speed;
        }
    }

    /// <summary>
    /// Called when the ball touches the killzone.
    /// Triggers reflect or destruction depending on powerup.
    /// </summary>
    public void OnHitKillzone()
    {
        if (hasKillzoneReflectPowerup)
        {
            hasKillzoneReflectPowerup = false;
            Vector2 velocity = rigidbody.linearVelocity;
            rigidbody.linearVelocity = new Vector2(velocity.x, -velocity.y);
            Debug.Log("Killzone reflect triggered!");
        }
        else
        {
            Destroy(gameObject);
            GameManager.Instance.OnBallDestroyed();
        }
    }

    // Magnet effect toggles
    public void StartMagnetEffect() { magnetActive = true; Debug.Log("Magnet effect started!"); }
    public void StopMagnetEffect() { magnetActive = false; Debug.Log("Magnet effect ended!"); }

    // Homing effect toggles
    public void StartHomingEffect() { homingActive = true; Debug.Log("Homing effect started!"); }
    public void StopHomingEffect() { homingActive = false; Debug.Log("Homing effect ended!"); }

    /// <summary>
    /// Collision logic for walls, bricks, and paddle. Triggers sound and effects.
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
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

    /// <summary>
    /// Initializes a ball that is spawned mid-play (e.g., Multiply effect).
    /// </summary>
    public void InitializeAsSpawnedBall(Vector2 velocity)
    {
        attachedToPaddle = false;
        isLaunched = true;
        rigidbody.linearVelocity = velocity;

        Debug.Log("Spawned ball with velocity: " + velocity);
    }
}
