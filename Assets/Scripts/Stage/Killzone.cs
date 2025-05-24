using UnityEngine;

/// <summary>
/// Detects when the ball enters the killzone and notifies the ball to handle the event.
/// </summary>
public class KillzoneTrigger : MonoBehaviour
{
    /// <summary>
    /// Called when another collider enters this trigger zone.
    /// If the object is a Ball, its OnHitKillzone behavior is triggered.
    /// </summary>
    /// <param name="other">The collider that entered the trigger.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        Ball ball = other.GetComponent<Ball>();
        if (ball != null)
        {
            ball.OnHitKillzone(); // ✅ Handle killzone logic inside the Ball script
        }
    }
}
