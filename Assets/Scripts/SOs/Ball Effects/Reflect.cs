using UnityEngine;

[CreateAssetMenu(menuName = "BallEffects/Reflect")]
public class Reflect : BallEffect
{
    public Sprite Icon;
    public override void OnHitPaddle(Ball ball, Paddle paddle)
    {
        if (Random.value < 0.05f)
        {
            ball.hasKillzoneReflectPowerup = true;
            Debug.Log("Reflect powerup granted!");
        }
    }
}
