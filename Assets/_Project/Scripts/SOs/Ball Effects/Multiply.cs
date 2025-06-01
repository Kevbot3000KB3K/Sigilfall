using UnityEngine;

[CreateAssetMenu(menuName = "BallEffects/Multiply")]
public class Multiply : BallEffect
{
    public GameObject ballPrefab;
    public AudioClip multiplySFX;
    public GameObject uiFeedbackPrefab;

    public override void OnHitBrick(Ball ball, Brick brick)
    {
        if (Random.value < 0.05f && ballPrefab != null)
        {
            GameObject newBallObj = GameObject.Instantiate(ballPrefab, ball.transform.position, Quaternion.identity);
            Ball newBall = newBallObj.GetComponent<Ball>();
            if (newBall != null)
            {
                newBall.skipInitialReset = true;
                newBall.hasKillzoneReflectPowerup = false;
                newBall.damage = ball.damage;
                newBall.activeEffects = new System.Collections.Generic.List<BallEffect>(ball.activeEffects);

                Vector2 originalVelocity = ball.rigidbody.linearVelocity;
                float angle = Random.Range(25f, 45f) * (Random.value < 0.5f ? 1f : -1f);
                Quaternion rotation = Quaternion.Euler(0, 0, angle);
                Vector2 newDirection = rotation * originalVelocity.normalized;

                newBall.InitializeAsSpawnedBall(newDirection * ball.speed);
            }

            // ✅ Play SFX
            if (multiplySFX != null)
            {
                AudioSource.PlayClipAtPoint(multiplySFX, ball.transform.position);
            }

            // ✅ Show UI feedback
            if (uiFeedbackPrefab != null)
            {
                GameObject hud = GameObject.Find("HUDCanvas"); // Make sure it exists in your scene
                if (hud != null)
                {
                    GameObject feedback = GameObject.Instantiate(uiFeedbackPrefab, hud.transform);
                    BallEffectUIFeedback feedbackScript = feedback.GetComponent<BallEffectUIFeedback>();
                    feedbackScript.Initialize(this.effectIcon);
                }
            }
        }
    }
}
