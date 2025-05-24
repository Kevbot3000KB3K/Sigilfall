using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the temporary visual feedback UI when a ball effect activates,
/// such as showing an icon that grows and fades out.
/// </summary>
public class BallEffectUIFeedback : MonoBehaviour
{
    [Header("UI Components")]
    public Image icon;                    // Icon to display for the effect
    public CanvasGroup canvasGroup;      // Used to fade the icon out

    [Header("Animation Settings")]
    public float duration = 0.8f;        // How long the animation lasts
    public float scaleMultiplier = 3f;   // How much the icon scales up

    private float timeElapsed = 0f;
    private Vector3 originalScale;

    /// <summary>
    /// Cache the original scale on start.
    /// </summary>
    private void Start()
    {
        originalScale = transform.localScale;
    }

    /// <summary>
    /// Initializes the feedback animation with the given effect icon.
    /// Resets scale and alpha for replayability.
    /// </summary>
    /// <param name="effectSprite">The sprite to display for the effect.</param>
    public void Initialize(Sprite effectSprite)
    {
        icon.sprite = effectSprite;
        canvasGroup.alpha = 1f;
        timeElapsed = 0f;
        transform.localScale = originalScale;
    }

    /// <summary>
    /// Updates the animation each frame — scaling up and fading out.
    /// Destroys the object after animation completes.
    /// </summary>
    private void Update()
    {
        timeElapsed += Time.deltaTime;
        float t = timeElapsed / duration;

        // Smoothly scale the icon up
        transform.localScale = Vector3.Lerp(originalScale, originalScale * scaleMultiplier, t);

        // Gradually fade the icon out
        canvasGroup.alpha = 1f - t;

        // Destroy when animation finishes
        if (t >= 1f)
        {
            Destroy(gameObject);
        }
    }
}
