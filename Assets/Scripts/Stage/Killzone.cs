using UnityEngine;

public class KillzoneTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Ball ball = other.GetComponent<Ball>();
        if (ball != null)
        {
            ball.OnHitKillzone(); // ✅ Trigger the destruction and GameManager tracking
        }
    }
}
