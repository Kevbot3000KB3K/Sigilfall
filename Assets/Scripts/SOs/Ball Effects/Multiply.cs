using UnityEngine;

[CreateAssetMenu(menuName = "BallEffects/Multiply")]
public class Multiply : BallEffect
{
    public GameObject ballPrefab; // Assign your Ball prefab here in the inspector
    public Sprite Icon;

    public override void OnHitBrick(Ball ball, Brick brick)
    {
        if (Random.value < 0.05f && ballPrefab != null)
        {
            // Spawn a duplicate ball at the current ball's position with some slight offset
            Vector3 spawnPos = ball.transform.position + new Vector3(0.2f, 0.2f, 0);
            GameObject newBall = Instantiate(ballPrefab, spawnPos, Quaternion.identity);

            // Optionally, copy velocity and launch the new ball
            Ball newBallScript = newBall.GetComponent<Ball>();
            if (newBallScript != null)
            {
                newBallScript.Launch();
            }
        }
    }
}
