using UnityEngine;

/// <summary>
/// Lifegain effect: When the ball hits a brick, there's a chance to grant the player an extra life.
/// Includes optional sound and visual feedback.
/// </summary>
[CreateAssetMenu(menuName = "Ball Effects/Lifegain")]
public class LifegainEffect : BallEffect
{
    [Header("Effect Chance")]
    [Tooltip("Chance (0 to 1) to gain a life when the ball hits a brick.")]
    [Range(0f, 1f)]
    public float lifegainChance = 0.5f;

    [Header("Visual & Audio Feedback")]
    [Tooltip("Icon used for UI effect feedback (optional).")]
    public Sprite Icon;

    [Tooltip("Visual effect prefab to play when gaining a life.")]
    public GameObject lifegainVFXPrefab;

    [Tooltip("Sound effect to play when gaining a life.")]
    public AudioClip lifegainSFX;

    /// <summary>
    /// Called when the ball hits a brick. Has a chance to grant the player a life.
    /// </summary>
    /// <param name="ball">The ball that triggered the effect.</param>
    /// <param name="brick">The brick that was hit.</param>
    public override void OnHitBrick(Ball ball, Brick brick)
    {
        // 🎲 Roll chance
        if (Random.value < lifegainChance)
        {
            // ❤️ Add life via GameManager
            GameManager.Instance.GainLife();

            // ✨ Play visual effect
            if (lifegainVFXPrefab != null)
            {
                GameObject vfx = Instantiate(lifegainVFXPrefab, ball.transform.position, Quaternion.identity);
                Destroy(vfx, 2f); // Cleanup after 2 seconds (optional tweak)
            }

            // 🔊 Play sound effect at ball's position
            if (lifegainSFX != null)
            {
                AudioSource.PlayClipAtPoint(lifegainSFX, ball.transform.position);
            }

            // 🖼️ Optional UI effect
            // BallEffectUIUtility.Instance?.PlayEffectFeedback(Icon);
        }
    }
}
