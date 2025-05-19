using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Ball Effects/Random Angle")]
public class RandomAngleEffect : BallEffect
{
    public float chance = 0.25f;
    public Sprite Icon;

    public override void OnHitBrick(Ball ball, Brick brick)
    {
        TryRandomize(ball);
    }

    public override void OnHitPaddle(Ball ball, Paddle paddle)
    {
        TryRandomize(ball);
    }

    private void TryRandomize(Ball ball)
    {
        if (Random.value < chance)
        {
            float angle = Random.Range(0, 360);
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
            ball.rigidbody.linearVelocity = direction * ball.speed;
        }
    }
}