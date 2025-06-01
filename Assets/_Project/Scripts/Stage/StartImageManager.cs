using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Displays a "Start!" image briefly when a stage loads before gameplay begins.
/// </summary>
public class StartImageManager : MonoBehaviour
{
    public GameObject startImage; // Assign the UI "Start!" image in Inspector

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (startImage != null)
            startImage.SetActive(false);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// Triggered when a new scene loads. Shows start image if it's a game stage.
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Update this string check to match your actual stage naming convention
        if (scene.name.StartsWith("Stage"))
        {
            Invoke(nameof(ShowStartImage), 0.1f); // Small delay to ensure UI is ready
        }
        else
        {
            if (startImage != null)
                startImage.SetActive(false);
        }
    }

    /// <summary>
    /// Shows the start image and sets up a timed fade-out.
    /// </summary>
    private void ShowStartImage()
    {
        if (startImage == null)
        {
            Debug.LogWarning("Start image is not assigned!");
            return;
        }

        startImage.SetActive(true);
        Invoke(nameof(FadeOutStartImage), 2f);
    }

    /// <summary>
    /// Hides the start image and launches the ball.
    /// </summary>
    private void FadeOutStartImage()
    {
        if (startImage != null)
            startImage.SetActive(false);

        if (GameManager.Instance != null && GameManager.Instance.ball != null)
        {
            GameManager.Instance.ball.Launch();
        }
        else
        {
            Debug.LogWarning("⚠️ Ball or GameManager not ready when trying to launch.");
        }
    }
}
