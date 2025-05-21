using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "BallEffects/Magnet")]
public class Magnet : BallEffect
{
    public AudioClip magnetSFX;
    public GameObject magnetEffectPrefab;
    public Sprite Icon;
    public override void OnHitBrick(Ball ball, Brick brick)
    {
        if (Random.value < 0.10f)
        {
            ball.ApplyEffectTemporary(
                effectStart: b =>
                {
                    b.StartMagnetEffect();

                    if (magnetSFX != null && b.TryGetComponent<AudioSource>(out var audio))
                        audio.PlayOneShot(magnetSFX);

                    if (magnetEffectPrefab != null)
                    {
                        GameObject fx = Instantiate(magnetEffectPrefab, b.transform.position, Quaternion.identity, b.transform);
                        GameObject.Destroy(fx, 5f);
                    }
                },
                effectEnd: b => b.StopMagnetEffect(),
                duration: 5f
            );
        }
    }
}