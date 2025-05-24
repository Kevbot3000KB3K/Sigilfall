using UnityEngine;

/// <summary>
/// Controls the behavior of a single brick, including health, visuals, and destruction logic.
/// </summary>
public class Brick : MonoBehaviour
{
    [Header("Brick Settings")]
    public Sprite[] states;                          // Different sprite visuals for each damage state
    public int points = 1;                           // Points awarded when destroyed
    public bool unbreakable;                         // If true, cannot be damaged or destroyed

    [Header("Effects")]
    public GameObject shatterEffectPrefab;           // Optional particle effect on destruction
    public AudioClip breakSound;                     // Optional sound effect on break

    public int health { get; private set; }          // Current health of the brick
    public SpriteRenderer spriteRenderer { get; private set; }

    /// <summary>
    /// Cache reference to SpriteRenderer.
    /// </summary>
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Initialize/reset brick state.
    /// </summary>
    private void Start()
    {
        ResetBrick();
    }

    /// <summary>
    /// Resets the brick to its starting state.
    /// </summary>
    public void ResetBrick()
    {
        gameObject.SetActive(true);

        if (!unbreakable)
        {
            health = states.Length;
            spriteRenderer.sprite = states[health - 1];
        }
    }

    /// <summary>
    /// Applies damage to the brick and updates its appearance or destroys it.
    /// </summary>
    /// <param name="damage">Amount of damage to apply.</param>
    public void TakeDamage(float damage = 1)
    {
        if (unbreakable)
            return;

        int intDamage = Mathf.CeilToInt(damage);

        for (int i = 0; i < intDamage; i++)
        {
            Hit();
            if (!gameObject.activeSelf) break;
        }
    }

    /// <summary>
    /// Internal function to handle being hit once.
    /// </summary>
    private void Hit()
    {
        if (unbreakable)
            return;

        health--;

        if (health <= 0)
        {
            if (breakSound != null)
                AudioSource.PlayClipAtPoint(breakSound, transform.position);

            if (shatterEffectPrefab != null)
                Instantiate(shatterEffectPrefab, transform.position, Quaternion.identity);

            gameObject.SetActive(false);
        }
        else
        {
            spriteRenderer.sprite = states[health - 1];
        }

        GameManager gm = FindFirstObjectByType<GameManager>();
        if (gm != null)
        {
            gm.Hit(this);
        }
    }

    /// <summary>
    /// Detects ball collision and applies damage.
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Ball ball = collision.gameObject.GetComponent<Ball>();
        if (ball != null)
        {
            TakeDamage(ball.damage);
        }
    }
}
