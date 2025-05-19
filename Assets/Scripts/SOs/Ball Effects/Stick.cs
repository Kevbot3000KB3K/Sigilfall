using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Ball Effects/Stick")]
public class StickEffect : BallEffect
{
    public float chance = 0.3f;
    public Sprite Icon;

    public override void OnHitPaddle(Ball ball, Paddle paddle)
    {
        if (Random.value < chance)
        {
            ball.StickToPaddle();
        }
    }
}