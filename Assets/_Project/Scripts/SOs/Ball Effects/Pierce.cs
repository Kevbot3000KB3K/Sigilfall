using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Ball Effects/Pierce")]
public class PierceEffect : BallEffect
{
    public float pierceChance = 0.25f;
    public Sprite Icon;

    public override void OnHitBrick(Ball ball, Brick brick)
    {
        if (Random.value < pierceChance)
        {
            brick.TakeDamage(ball.damage);
            Physics2D.IgnoreCollision(ball.GetComponent<Collider2D>(), brick.GetComponent<Collider2D>(), true);
        }
    }
}