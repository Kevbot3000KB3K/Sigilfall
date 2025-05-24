using UnityEngine;

/// <summary>
/// Magnet effect: When the ball hits a brick, it has a chance to activate a magnetic pull
/// toward the paddle, allowing the player to influence its path. Includes optional visual and audio feedback.
/// </summary>
[CreateAssetMenu(menuName = "BallEffects/Magnet")]
public class Magnet : BallEffect
{
    [Header("Effect Settings")]
    [Tooltip("Duration of the magnetic effect in seconds.")]
    public float magnetDuration = 5f;

    [Tooltip("Chance (0 to 1) for the effect to trigger upon hitting a brick.")]
    public float activationChance = 0.10f;

    [Header("Audio & Visual Feedback")]
    [Tooltip("Sound effect played when the magnet effect activates.")]
    public AudioClip magnetSFX;

    [Tooltip("Optional visual effect prefab to show magnetism.")]
    public GameObject magnetEffectPrefab;

    [Tooltip("Icon used for UI feedback (optional).")]
    public Sprite Icon;

    /// <summary>
    /// Triggers when the ball hits a brick. Has a chance to start a magnetic effect.
    /// </summary>
    /// <param name="ball">The ball that hit the brick.</param>
    /// <param name="brick">The brick that was hit.</param>
    public override void OnHitBrick(Ball ball, Brick brick)
    {
        // 🎲 Random chance to activate magnetism
        if (Random.value < activationChance)
        {
            ball.ApplyEffectTemporary(
                effectStart: b =>
                {
                    b.StartMagnetEffect();

                    // 🔊 Play audio feedback
                    if (magnetSFX != null && b.TryGetComponent<AudioSource>(out var audio))
                        audio.PlayOneShot(magnetSFX);

                    // 💫 Spawn visual effect
                    if (magnetEffectPrefab != null)
                    {
                        GameObject fx = Instantiate(magnetEffectPrefab, b.transform.position, Quaternion.identity, b.transform);
                        Destroy(fx, magnetDuration); // Cleanup after duration
                    }

                    // 🖼️ Optional: UI feedback hook
                    // BallEffectUIUtility.Instance?.PlayEffectFeedback(Icon);
                },
                effectEnd: b =>
                {
                    b.StopMagnetEffect();
                },
                duration: magnetDuration
            );
        }
    }
}
