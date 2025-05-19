using UnityEngine;

[CreateAssetMenu(menuName = "BallEffects/Grow")]
public class Grow : BallEffect
{
    public float growDuration = 8f;
    public float growScale = 2f;
    public AudioClip growSound;
    public Sprite Icon;

    public override void OnHitPaddle(Ball ball, Paddle paddle)
    {
        if (Random.value < 0.10f)
        {
            ball.ApplyEffectTemporary(
                effectStart: b =>
                {
                    b.transform.localScale = Vector3.one * growScale;
                    var collider = b.GetComponent<CircleCollider2D>();
                    if (collider != null)
                        collider.radius *= growScale;
                },
                effectEnd: b =>
                {
                    b.transform.localScale = Vector3.one;
                    var collider = b.GetComponent<CircleCollider2D>();
                    if (collider != null)
                        collider.radius /= growScale;
                },
                duration: growDuration
            );
            if (growSound != null && ball.TryGetComponent<AudioSource>(out var audioSource))
            {
                audioSource.PlayOneShot(growSound);
            }
        }
    }
}
