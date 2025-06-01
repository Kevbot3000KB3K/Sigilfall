using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Handles confirmation and transition to the selected stage.
/// Plays fade-out audio and loads the stage scene.
/// </summary>
public class ConfirmStage : MonoBehaviour
{
    /// <summary>
    /// Name of the scene to load after confirmation.
    /// Set this before calling ConfirmStageSelection().
    /// </summary>
    private string selectedStageSceneName;

    /// <summary>
    /// Sets the selected stage name to be loaded later.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load.</param>
    public void SetSelectedStage(string sceneName)
    {
        selectedStageSceneName = sceneName;
        Debug.Log("🛰️ Selected stage: " + selectedStageSceneName);
    }

    /// <summary>
    /// Starts the transition to the selected stage if one has been set.
    /// </summary>
    public void ConfirmStageSelection()
    {
        if (!string.IsNullOrEmpty(selectedStageSceneName))
        {
            StartCoroutine(FadeAndLoadScene());
        }
        else
        {
            Debug.LogWarning("⚠️ No stage selected!");
        }
    }

    /// <summary>
    /// Fades out background music (if available) and loads the selected scene.
    /// </summary>
    private IEnumerator FadeAndLoadScene()
    {
        // 🔊 Fade music if audio manager is present
        if (PlanetSelectAudio.Instance != null)
        {
            PlanetSelectAudio.Instance.FadeOutMusic(1f);
            yield return new WaitForSeconds(1f); // Wait for fade-out
        }

        // 🚀 Load the selected stage
        SceneManager.LoadScene(selectedStageSceneName);
    }
}
