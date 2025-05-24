using UnityEngine;

[CreateAssetMenu(menuName = "Ball Effects/Lifegain")]
public class LifegainEffect : BallEffect
{
    [Range(0f, 1f)]
    public float lifegainChance = 0.5f;

    public Sprite Icon;
    public GameObject lifegainVFXPrefab; // Assign your animation prefab here
    public AudioClip lifegainSFX; // Assign your sound clip here

    public override void OnHitBrick(Ball ball, Brick brick)
    {
        if (Random.value < lifegainChance)
        {
            // Increase life
            GameManager.Instance.GainLife();

            // Play animation on top of the ball
            if (lifegainVFXPrefab != null)
            {
                GameObject vfx = GameObject.Instantiate(lifegainVFXPrefab, ball.transform.position, Quaternion.identity);
                // Do NOT parent it to the ball if you want it to stay still
                // vfx.transform.SetParent(ball.transform); <- REMOVE THIS LINE
            }


            // Play sound
            if (lifegainSFX != null)
            {
                AudioSource.PlayClipAtPoint(lifegainSFX, ball.transform.position);
            }
        }
    }
}
