using TMPro;
using UnityEngine;
using System.Collections;

/// <summary>
/// Manages the in-game UI, including score/lives display and the start message animation.
/// Optimized to only update the UI when values change.
/// </summary>
public class UIManager : MonoBehaviour
{
    [Tooltip("Reference to the text element displaying the player's score.")]
    public TextMeshProUGUI scoreText;

    [Tooltip("Reference to the text element displaying the player's remaining lives.")]
    public TextMeshProUGUI livesText;

    [Tooltip("CanvasGroup used to fade the start message in and out.")]
    public CanvasGroup startMessageCanvasGroup;

    private GameManager gameManager;

    private int lastScore = -1;
    private int lastLives = -1;

    /// <summary>
    /// Finds the GameManager, updates UI initially, and shows the start message.
    /// </summary>
    private void Start()
    {
        gameManager = GameManager.Instance;
        ForceUpdateUI(); // Ensures UI is initialized correctly
        StartCoroutine(ShowStartMessage());
    }

    /// <summary>
    /// Checks if score or lives changed and updates UI accordingly.
    /// </summary>
    private void Update()
    {
        if (gameManager == null) return;

        if (gameManager.score != lastScore || gameManager.lives != lastLives)
        {
            UpdateUI();
        }
    }

    /// <summary>
    /// Updates the UI and caches the current values.
    /// </summary>
    private void UpdateUI()
    {
        scoreText.text = gameManager.score.ToString();
        livesText.text = gameManager.lives.ToString();

        lastScore = gameManager.score;
        lastLives = gameManager.lives;
    }

    /// <summary>
    /// Forces a UI update regardless of previous values (e.g., on scene start).
    /// </summary>
    private void ForceUpdateUI()
    {
        lastScore = -1;
        lastLives = -1;
        UpdateUI();
    }

    /// <summary>
    /// Fades in the start message and then fades it out.
    /// </summary>
    private IEnumerator ShowStartMessage()
    {
        startMessageCanvasGroup.alpha = 0f;

        // Fade in
        float fadeDuration = 0.5f;
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            startMessageCanvasGroup.alpha = Mathf.Lerp(0f, 0.5f, elapsedTime / fadeDuration);
            yield return null;
        }

        startMessageCanvasGroup.alpha = 1f;

        yield return new WaitForSeconds(1f);

        // Fade out
        elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            startMessageCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            yield return null;
        }

        startMessageCanvasGroup.alpha = 0f;
    }
}
