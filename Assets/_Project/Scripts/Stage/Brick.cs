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
    [Header("Elemental Affinity")]
    public Family family;
    [Header("UI")]
    public GameObject damagePopupPrefab;

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
            health = 8; // Bricks now have 8 health points
            spriteRenderer.sprite = states[GetStateIndexFromHealth()];
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
            spriteRenderer.sprite = states[GetStateIndexFromHealth()];
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
            float calculatedDamage = ball.damage;

            if (ball.sigil != null && family != null)
            {
                calculatedDamage *= ball.sigil.GetModifierAgainst(family);
            }

            ShowDamagePopup(calculatedDamage); // <- new

            TakeDamage(calculatedDamage);
        }

    }

    private void ShowDamagePopup(float damage)
    {
        if (damagePopupPrefab != null)
        {
            GameObject popup = Instantiate(damagePopupPrefab, transform.position, Quaternion.identity);
            DamagePopup popupScript = popup.GetComponent<DamagePopup>();

            if (popupScript != null)
            {
                popupScript.Show(damage, transform.position + Vector3.up * 0.5f); // Pop above brick
            }
        }
    }



    /// <summary>
    /// Converts current health to the appropriate sprite index.
    /// </summary>
    private int GetStateIndexFromHealth()
    {
        if (health >= 8) return 4;
        else if (health >= 6) return 3;
        else if (health >= 4) return 2;
        else if (health >= 2) return 1;
        else return 0;
    }

}
