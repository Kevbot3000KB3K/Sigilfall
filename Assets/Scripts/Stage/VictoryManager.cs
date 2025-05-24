using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles the logic for displaying the victory screen,
/// playing fanfare, pausing the game, and proceeding to the next scene.
/// </summary>
public class VictoryManager : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("Panel shown when the player wins.")]
    public GameObject victoryPanel;

    [Tooltip("Button used to proceed to the next scene.")]
    public Button proceedButton;

    [Header("Audio")]
    [Tooltip("AudioSource used to play the victory fanfare.")]
    public AudioSource sfxSource;

    [Tooltip("AudioClip played when the player wins.")]
    public AudioClip victoryFanfare;

    [Header("Scene")]
    [Tooltip("The name of the scene to load when proceeding.")]
    public string nextScene = "StageSelect_Holy";

    /// <summary>
    /// Initializes the UI and assigns the proceed button listener.
    /// </summary>
    private void Start()
    {
        if (victoryPanel != null)
            victoryPanel.SetActive(false);

        if (proceedButton != null)
            proceedButton.onClick.AddListener(ProceedToNextScene);
    }

    /// <summary>
    /// Displays the victory screen, plays the sound, and pauses the ball.
    /// </summary>
    public void ShowVictory()
    {
        // Play audio
        if (sfxSource != null && victoryFanfare != null)
            sfxSource.PlayOneShot(victoryFanfare);

        // Show panel
        if (victoryPanel != null)
            victoryPanel.SetActive(true);

        // Pause the ball
        Ball ball = Object.FindFirstObjectByType<Ball>(); // ✅ Fix deprecated usage
        if (ball != null)
        {
            ball.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Loads the next scene as defined in the inspector.
    /// </summary>
    private void ProceedToNextScene()
    {
        SceneManager.LoadScene(nextScene);
    }
}
