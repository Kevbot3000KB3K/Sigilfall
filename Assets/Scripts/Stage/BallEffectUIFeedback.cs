using UnityEngine;
using UnityEngine.UI;

public class BallEffectUIFeedback : MonoBehaviour
{
    public Image icon;
    public CanvasGroup canvasGroup;
    public float duration = 0.8f;
    public float scaleMultiplier = 3f;

    private float timeElapsed = 0f;
    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
    }

    public void Initialize(Sprite effectSprite)
    {
        icon.sprite = effectSprite;
        canvasGroup.alpha = 1f;
        timeElapsed = 0f;
        transform.localScale = originalScale;
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;
        float t = timeElapsed / duration;

        // Scale up
        transform.localScale = Vector3.Lerp(originalScale, originalScale * scaleMultiplier, t);

        // Fade out
        canvasGroup.alpha = 1f - t;

        if (t >= 1f)
        {
            Destroy(gameObject);
        }
    }
}
