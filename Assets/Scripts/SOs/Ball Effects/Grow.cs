using UnityEngine;

[CreateAssetMenu(menuName = "BallEffects/Grow")]
public class Grow : BallEffect
{
    public float growDuration = 4f;
    public float growScale = 2f;
    public AudioClip growSound;

    public override void OnHitBrick(Ball ball, Brick brick)
    {
        if (ball.isGrown)
            return; // ✅ Skip if already grown

        if (Random.value < 0.10f)
        {
            float originalScale = ball.transform.localScale.x;
            float newScale = originalScale * growScale;

            CircleCollider2D collider = ball.GetComponent<CircleCollider2D>();
            float originalRadius = collider != null ? collider.radius : 0.5f;

            ball.isGrown = true; // ✅ Prevent re-triggering

            ball.ApplyEffectTemporary(
                effectStart: b =>
                {
                    b.transform.localScale = Vector3.one * newScale;
                    if (collider != null)
                        collider.radius = originalRadius; // transform already scales it
                },
                effectEnd: b =>
                {
                    b.transform.localScale = Vector3.one * originalScale;
                    if (collider != null)
                        collider.radius = originalRadius;

                    b.isGrown = false; // ✅ Reset when done
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
