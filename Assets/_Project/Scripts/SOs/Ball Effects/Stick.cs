using UnityEngine;

/// <summary>
/// StickEffect gives the ball a chance to stick to the paddle upon contact,
/// allowing the player to re-launch it. Includes optional audio and visual feedback.
/// </summary>
[CreateAssetMenu(menuName = "Ball Effects/Stick")]
public class StickEffect : BallEffect
{
    [Header("Effect Settings")]
    [Tooltip("Chance (0 to 1) that the ball will stick to the paddle.")]
    public float chance = 0.25f;

    [Header("Visual & Audio Feedback")]
    [Tooltip("Icon shown in the UI when this effect activates (optional).")]
    public Sprite Icon;

    [Tooltip("Sound effect played when the ball sticks.")]
    public AudioClip stickSound;

    [Tooltip("Optional visual prefab that spawns when the ball sticks.")]
    public GameObject stickEffectPrefab;

    /// <summary>
    /// Called when the ball hits the paddle. Triggers the stick effect based on chance.
    /// </summary>
    /// <param name="ball">The ball that hit the paddle.</param>
    /// <param name="paddle">The paddle that was hit.</param>
    public override void OnHitPaddle(Ball ball, Paddle paddle)
    {
        // 🎲 Random chance to activate
        if (Random.value < chance)
        {
            // Stick the ball to the paddle
            ball.StickToPaddle();

            // 🔊 Play sound if available
            if (stickSound != null && ball.TryGetComponent<AudioSource>(out var audio))
            {
                audio.PlayOneShot(stickSound);
            }

            // 💫 Spawn optional visual effect
            if (stickEffectPrefab != null)
            {
                GameObject effect = Instantiate(stickEffectPrefab, ball.transform.position, Quaternion.identity);
                Destroy(effect, 1f);
            }

            // 🖼️ Optional: trigger UI feedback animation
            // BallEffectUIUtility.Instance?.PlayEffectFeedback(Icon);
        }
    }
}
