using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class ConfirmStage : MonoBehaviour
{
    private string selectedStageSceneName;
    public void SetSelectedStage(string sceneName)
    {
        selectedStageSceneName = sceneName;
        Debug.Log("Selected stage: " + selectedStageSceneName);
    }

    public void ConfirmStageSelection()
    {
        if (!string.IsNullOrEmpty(selectedStageSceneName))
        {
            StartCoroutine(FadeAndLoadScene());
        }
        else
        {
            Debug.LogWarning("No stage selected!");
        }
    }

    private IEnumerator FadeAndLoadScene()
    {
        if (PlanetSelectAudio.Instance != null)
        {
            PlanetSelectAudio.Instance.FadeOutMusic(1f);
            yield return new WaitForSeconds(1f);
        }
        SceneManager.LoadScene(selectedStageSceneName);
    }
}