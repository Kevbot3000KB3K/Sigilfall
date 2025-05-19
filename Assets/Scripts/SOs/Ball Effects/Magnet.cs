using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "BallEffects/Magnet")]
public class Magnet : BallEffect
{
    public Sprite Icon;
    public override void OnHitBrick(Ball ball, Brick brick)
    {
        if (Random.value < 0.20f)
        {
            ball.ApplyEffectTemporary(
                effectStart: b => b.StartMagnetEffect(),
                effectEnd: b => b.StopMagnetEffect(),
                duration: 5f
            );
        }
    }
}