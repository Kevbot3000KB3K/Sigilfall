using UnityEngine;

public abstract class BallEffect : ScriptableObject
{
    [Header("Basic Info")]
    public string effectName;
    public string description;
    public Sprite effectIcon;
    [Header("Optional: Duration-based Effects")]
    public bool isTemporary = false;
    public float duration = 5f;

    // Track internal timers if needed
    protected float timeRemaining;

    public virtual void Activate(Ball ball)
    {
        if (isTemporary)
            timeRemaining = duration;
    }

    // Called when the ball hits a brick
    public virtual void OnHitBrick(Ball ball, Brick brick) { }

    // Called when the ball hits the paddle
    public virtual void OnHitPaddle(Ball ball, Paddle paddle) { }

    // Called every frame (for things like timers or teleport logic)
    public virtual void OnUpdate(Ball ball)
    {
        if (isTemporary)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining <= 0)
                OnExpire(ball);
        }
    }

    // Called when the effect ends (only if temporary)
    public virtual void OnExpire(Ball ball) { }
}
