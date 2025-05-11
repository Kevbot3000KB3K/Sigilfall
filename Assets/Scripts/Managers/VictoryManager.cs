using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VictoryManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject victoryPanel;
    public Button proceedButton;

    [Header("Audio")]
    public AudioSource sfxSource;
    public AudioClip victoryFanfare;

    [Header("Scene")]
    public string nextScene = "StageSelect_Holy"; 

    private void Start()
    {
        if (victoryPanel != null)
            victoryPanel.SetActive(false); 

        if (proceedButton != null)
            proceedButton.onClick.AddListener(ProceedToNextScene);
    }

    public void ShowVictory()
    {
        // Play sound
        if (sfxSource != null && victoryFanfare != null)
            sfxSource.PlayOneShot(victoryFanfare);

        // Show UI
        if (victoryPanel != null)
            victoryPanel.SetActive(true);

        // Pause the ball
        Ball ball = FindObjectOfType<Ball>();
        if (ball != null)
        {
            ball.gameObject.SetActive(false); 
        }
    }

    private void ProceedToNextScene()
    {
        SceneManager.LoadScene(nextScene);
    }
}
