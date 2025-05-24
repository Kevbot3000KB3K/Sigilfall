using UnityEngine;

/// <summary>
/// Base class for all ball effects. Inherit from this to create unique ScriptableObject-based effects
/// that respond to in-game events like hitting bricks, paddles, or updating over time.
/// </summary>
public abstract class BallEffect : ScriptableObject
{
    [Header("Basic Info")]
    [Tooltip("Name of the effect (used in UI/debug).")]
    public string effectName;

    [Tooltip("Short description of what the effect does.")]
    [TextArea]
    public string description;

    [Tooltip("Icon used to represent this effect in UI (optional).")]
    public Sprite effectIcon;

    [Header("Optional: Timed Effects")]
    [Tooltip("If true, the effect will expire after a duration.")]
    public bool isTemporary = false;

    [Tooltip("Duration the effect lasts if temporary.")]
    public float duration = 5f;

    /// <summary>
    /// Internal timer used to track temporary effect duration.
    /// </summary>
    protected float timeRemaining;

    /// <summary>
    /// Called when the effect is first applied to the ball.
    /// </summary>
    public virtual void Activate(Ball ball)
    {
        if (isTemporary)
            timeRemaining = duration;
    }

    /// <summary>
    /// Called when the ball hits a brick. Override to define custom behavior.
    /// </summary>
    public virtual void OnHitBrick(Ball ball, Brick brick) { }

    /// <summary>
    /// Called when the ball hits the paddle. Override to define custom behavior.
    /// </summary>
    public virtual void OnHitPaddle(Ball ball, Paddle paddle) { }

    /// <summary>
    /// Called every frame if the effect is active. Handles timer updates for temporary effects.
    /// </summary>
    public virtual void OnUpdate(Ball ball)
    {
        if (isTemporary)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining <= 0)
                OnExpire(ball);
        }
    }

    /// <summary>
    /// Called when the effect's duration expires (if it's temporary).
    /// Override for cleanup logic.
    /// </summary>
    public virtual void OnExpire(Ball ball) { }
}
