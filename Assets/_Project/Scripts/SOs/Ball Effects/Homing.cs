using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "BallEffects/Homing")]
public class Homing : BallEffect
{
    public Sprite Icon;
    public override void OnHitBrick(Ball ball, Brick brick)
    {
        if (Random.value < 0.20f)
        {
            ball.ApplyEffectTemporary(
                effectStart: b => b.StartHomingEffect(),
                effectEnd: b => b.StopHomingEffect(),
                duration: 5f
            );
        }
    }
}
