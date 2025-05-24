using UnityEngine;
using System.Collections;

/// <summary>
/// Grow ball effect: When triggered, the ball smoothly scales up for a short time,
/// increasing its size and collider. After the duration, it smoothly scales back to normal.
/// </summary>
[CreateAssetMenu(menuName = "BallEffects/Grow")]
public class Grow : BallEffect
{
    [Header("Grow Settings")]
    [Tooltip("How long the grow effect lasts in seconds.")]
    public float growDuration = 4f;

    [Tooltip("Multiplier applied to the ball's size (e.g., 2 = double size).")]
    public float growScale = 2f;

    [Tooltip("Optional sound effect to play when the ball grows.")]
    public AudioClip growSound;

    /// <summary>
    /// Called when the ball hits a brick. 10% chance to trigger the Grow effect.
    /// </summary>
    public override void OnHitBrick(Ball ball, Brick brick)
    {
        if (ball.isGrown)
            return;

        if (Random.value < 0.10f)
        {
            Vector3 originalScale = ball.transform.localScale;
            Vector3 targetScale = originalScale * growScale;

            CircleCollider2D collider = ball.GetComponent<CircleCollider2D>();
            float originalRadius = collider != null ? collider.radius : 0.5f;
            float targetRadius = originalRadius * growScale;

            ball.isGrown = true;

            // Animate scale up
            ball.StartCoroutine(ScaleAndResizeCollider(ball.transform, collider, originalScale, targetScale, originalRadius, targetRadius, 0.15f));

            ball.ApplyEffectTemporary(
                effectStart: b => { },
                effectEnd: b =>
                {
                    // Animate scale and collider back down
                    b.StartCoroutine(ScaleAndResizeCollider(b.transform, collider, b.transform.localScale, originalScale, targetRadius, originalRadius, 0.15f));
                    b.isGrown = false;
                },
                duration: growDuration
            );

            if (growSound != null && ball.TryGetComponent<AudioSource>(out var audio))
            {
                audio.PlayOneShot(growSound);
            }
        }
    }

    /// <summary>
    /// Smoothly scales the ball and updates collider radius over time.
    /// </summary>
    private IEnumerator ScaleAndResizeCollider(Transform target, CircleCollider2D collider, Vector3 startScale, Vector3 endScale, float startRadius, float endRadius, float duration)
    {
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float lerpT = Mathf.Clamp01(t / duration);
            target.localScale = Vector3.Lerp(startScale, endScale, lerpT);

            if (collider != null)
                collider.radius = Mathf.Lerp(startRadius, endRadius, lerpT);

            yield return null;
        }

        target.localScale = endScale;

        if (collider != null)
            collider.radius = endRadius;
    }
}
