using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Ball Effects/Explode")]
public class ExplodeEffect : BallEffect
{
    public float explodeChance = 0.1f;
    public float explosionRadius = 1.5f;
    public Sprite Icon;

    public override void OnHitBrick(Ball ball, Brick brick)
    {
        if (Random.value < explodeChance)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(brick.transform.position, explosionRadius);
            foreach (var hit in hits)
            {
                Brick nearbyBrick = hit.GetComponent<Brick>();
                if (nearbyBrick != null && nearbyBrick != brick)
                    nearbyBrick.TakeDamage(ball.damage);
            }
        }
    }
}