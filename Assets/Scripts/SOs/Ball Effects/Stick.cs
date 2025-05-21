using UnityEngine;

[CreateAssetMenu(menuName = "Ball Effects/Stick")]
public class StickEffect : BallEffect
{
    public float chance = 0.25f;
    public Sprite Icon;
    public AudioClip stickSound;
    public GameObject stickEffectPrefab; // Optional visual

    public override void OnHitPaddle(Ball ball, Paddle paddle)
    {
        if (Random.value < chance)
        {
            ball.StickToPaddle();

            if (stickSound != null && ball.TryGetComponent<AudioSource>(out var audio))
            {
                audio.PlayOneShot(stickSound);
            }

            if (stickEffectPrefab != null)
            {
                GameObject effect = Instantiate(stickEffectPrefab, ball.transform.position, Quaternion.identity);
                Destroy(effect, 1f);
            }

            // Optional: UI feedback
            // BallEffectUIUtility.Instance?.PlayEffectFeedback(this.effectIcon);
        }
    }
}
