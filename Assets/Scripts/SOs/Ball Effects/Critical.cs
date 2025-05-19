using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Ball Effects/Critical")]
public class CriticalEffect : BallEffect
{
    public float criticalChance = 0.25f;
    public Sprite Icon;
    public AudioClip criticalHitSound;


    public override void OnHitBrick(Ball ball, Brick brick)
    {
        if (Random.value < criticalChance)
        {
            brick.TakeDamage(ball.damage * 2);
            if (criticalHitSound != null && ball.TryGetComponent<AudioSource>(out var audioSource))
            {
                audioSource.PlayOneShot(criticalHitSound);
            }
        }
    }
}
