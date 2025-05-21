using UnityEngine;

[CreateAssetMenu(menuName = "Ball Effects/Explode")]
public class ExplodeEffect : BallEffect
{
    public float explodeChance = 0.1f;
    public float explosionRadiusMultiplier = 4f;
    public AudioClip explosionSound;
    public GameObject explosionEffectPrefab; // Assign your animation prefab here
    public Sprite Icon;

    public override void OnHitBrick(Ball ball, Brick brick)
    {
        if (Random.value < explodeChance)
        {
            float radius = ball.GetComponent<CircleCollider2D>().radius * ball.transform.localScale.x * explosionRadiusMultiplier;

            // ✅ Damage nearby bricks
            Collider2D[] hits = Physics2D.OverlapCircleAll(brick.transform.position, radius);
            foreach (var hit in hits)
            {
                Brick nearby = hit.GetComponent<Brick>();
                if (nearby != null && nearby.gameObject.activeSelf)
                {
                    nearby.TakeDamage(ball.damage);
                }
            }

            // ✅ Play explosion animation
            if (explosionEffectPrefab != null)
            {
                GameObject effect = GameObject.Instantiate(explosionEffectPrefab, brick.transform.position, Quaternion.identity);
                GameObject.Destroy(effect, 1f); // cleanup after 1s if needed
            }

            // ✅ Play sound
            if (explosionSound != null)
            {
                AudioSource.PlayClipAtPoint(explosionSound, brick.transform.position);
            }

            // ✅ Optional: UI feedback
            // BallEffectUIUtility.Instance?.PlayEffectFeedback(this.effectIcon);
        }
    }
}
