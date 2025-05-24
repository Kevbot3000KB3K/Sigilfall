using TMPro;
using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI livesText;
    public CanvasGroup startMessageCanvasGroup; 

    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        UpdateUI();
        StartCoroutine(ShowStartMessage());
    }

    private void Update()
    {
        UpdateUI(); 
    }

    private void UpdateUI()
    {
        if (gameManager != null)
        {
            scoreText.text = "" + gameManager.score;
            livesText.text = "" + gameManager.lives;
        }
    }

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
