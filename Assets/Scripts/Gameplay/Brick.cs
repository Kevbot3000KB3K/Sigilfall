using UnityEngine;

public class Brick : MonoBehaviour
{
    public SpriteRenderer spriteRenderer { get; private set; }
    public Sprite[] states;
    public int health { get; private set; }
    public int points = 1;
    public bool unbreakable;
    public GameObject shatterEffectPrefab;
    public AudioClip breakSound;

    private void Awake()
    {
        this.spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        ResetBrick();
    }

    public void ResetBrick()
    {
        this.gameObject.SetActive(true);

        if (!this.unbreakable)
        {
            this.health = this.states.Length;
            this.spriteRenderer.sprite = this.states[this.health - 1];
        }
    }

    public void TakeDamage(float damage = 1)
    {
        if (unbreakable)
            return;

        // Apply damage
        int intDamage = Mathf.CeilToInt(damage); // Round up to ensure visible effect
        for (int i = 0; i < intDamage; i++)
        {
            Hit(); // Existing logic handles sprite changes and deactivation
            if (!gameObject.activeSelf) break; // Exit early if destroyed
        }
    }


    private void Hit()
    {
        if (this.unbreakable)
            return;

        this.health--;

        if (this.health <= 0)
        {
            if (breakSound != null)
            {
                AudioSource.PlayClipAtPoint(breakSound, transform.position);
            }

            // Instantiate visual effect
            if (shatterEffectPrefab != null)
            {
                Instantiate(shatterEffectPrefab, transform.position, Quaternion.identity);
            }

            this.gameObject.SetActive(false);
        }
        else
        {
            this.spriteRenderer.sprite = this.states[this.health - 1];
        }
        FindFirstObjectByType<GameManager>().Hit(this);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Ball ball = collision.gameObject.GetComponent<Ball>();
        if (ball != null)
        {
            TakeDamage(ball.damage); // ✅ Apply actual ball damage
        }
    }



}
