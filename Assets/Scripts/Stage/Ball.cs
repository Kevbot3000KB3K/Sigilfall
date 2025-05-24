using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;


public class Ball : MonoBehaviour
{
    public new Rigidbody2D rigidbody { get; private set; }
    public float speed = 500f;
    public float distanceFromPaddle = 1f;
    public float damage = 1f; // Default damage for a regular ball hit
    private bool isStuckToPaddle = false;

    public List<BallEffect> activeEffects;
    public bool attachedToPaddle = true; // NEW: Needed for Multiply effect

    public bool skipInitialReset = false;

    public AudioClip wallHitSFX;
    public AudioClip brickHitSFX;
    public AudioClip paddleHitSFX;
    public AudioClip launchSFX;
    private AudioSource audioSource;
    private bool homingActive = false;
    public bool isLaunched { get; set; }

    private Transform paddle;
    private readonly List<IEnumerator> temporaryEffectCoroutines = new();
    public bool isGrown = false;

    private bool magnetActive = false;
    public bool hasKillzoneReflectPowerup = false;
    private void Awake()
    {
        this.rigidbody = GetComponent<Rigidbody2D>();
        paddle = GameObject.FindGameObjectWithTag("Paddle").transform;  
    }

    private void Start()
    {
        if (!skipInitialReset)
        {
            ResetBall();
        }
        GameManager.Instance.RegisterBall(); // Must happen in Start


        audioSource = GetComponent<AudioSource>();

        if (PlayerData.Instance != null && PlayerData.Instance.HasEquippedSigil())
        {
            Sigil mySigil = PlayerData.Instance.equippedSigil;

            // 🎨 Visual customization
            GetComponent<SpriteRenderer>().sprite = mySigil.sigilSprite;
            var trail = GetComponent<TrailRenderer>();
            trail.startColor = mySigil.trailColor;
            trail.endColor = mySigil.trailColor;

            // 🔥 Apply special effect
            if (mySigil.special != null)
            {
                if (activeEffects == null)
                    activeEffects = new List<BallEffect>();

                activeEffects.Add(mySigil.special);
            }
        }

        // ✅ Remove any nulls from the list
        activeEffects = activeEffects.Where(effect => effect != null).ToList();
    }



    public void ResetBall()
    {
        this.transform.position = paddle.position + new Vector3(0, distanceFromPaddle, 0);
        this.rigidbody.linearVelocity = Vector2.zero;
        isLaunched = false;
        attachedToPaddle = true;

        var trail = GetComponent<TrailRenderer>();
        if (trail != null)
        {
            trail.Clear();         // Clean slate
            trail.enabled = false; // No trail before launch
        }
    }


    public void Launch()
    {
        if (!isLaunched)
        {
            isLaunched = true;
            attachedToPaddle = false;

            SetRandomTrajectory();

            var trail = GetComponent<TrailRenderer>();
            if (trail != null)
            {
                trail.Clear();
                trail.enabled = true;
            }

            // ✅ Play launch SFX
            if (launchSFX != null && audioSource != null)
            {
                audioSource.PlayOneShot(launchSFX);
            }
        }
    }



    public void StickToPaddle()
    {
        attachedToPaddle = true;
        isLaunched = false;
        rigidbody.linearVelocity = Vector2.zero;

        // Disable trail while stuck
        var trail = GetComponent<TrailRenderer>();
        if (trail != null)
            trail.enabled = false;

        Debug.Log("Ball has stuck to paddle!");
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
        float y = Random.Range(0.5f, 1f); // ensure upward bias

        Vector2 direction = new Vector2(x, y).normalized;
        rigidbody.linearVelocity = direction * speed;
    }



    private void Update()
    {
        if (!isLaunched && attachedToPaddle)
        {
            transform.position = paddle.position + new Vector3(0, distanceFromPaddle, 0);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Launch();
            }
        }

        foreach (var effect in activeEffects)
        {
            effect.OnUpdate(this);
        }
        if (magnetActive && paddle != null)
        {
            Vector2 ballPos = transform.position;
            Vector2 paddleTarget = new Vector2(paddle.position.x, ballPos.y); // match x only
            transform.position = Vector2.Lerp(ballPos, paddleTarget, Time.deltaTime * 2.5f);
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
            Vector2 velocity = rigidbody.linearVelocity;
            rigidbody.linearVelocity = new Vector2(velocity.x, -velocity.y);
            Debug.Log("Killzone reflect triggered!");
        }
        else
        {
            Destroy(this.gameObject); // 💥 Remove the ball
            GameManager.Instance.OnBallDestroyed(); // 🧠 Notify manager
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
        if (!isLaunched) return;

        // Prevent ball from slowing or getting stuck
        if (isLaunched && rigidbody.linearVelocity.magnitude < 0.01f)
        {
            Debug.LogWarning("Ball velocity too low — reapplying upward nudge.");
            rigidbody.linearVelocity = new Vector2(Random.Range(-0.5f, 0.5f), 1f).normalized * speed;
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
    public void InitializeAsSpawnedBall(Vector2 velocity)
    {
        attachedToPaddle = false;
        isLaunched = true;
        rigidbody.linearVelocity = velocity;

        Debug.Log("Spawned ball with velocity: " + velocity);
    }


}
