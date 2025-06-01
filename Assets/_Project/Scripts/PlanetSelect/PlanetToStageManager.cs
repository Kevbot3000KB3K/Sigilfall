using UnityEngine;
using UnityEngine.SceneManagement;

public class PlanetToStageManager : MonoBehaviour
{
    public static PlanetToStageManager Instance;

    public string selectedPlanetName;

    public AudioClip errorSound;
    public AudioSource sfxSource;

    [Tooltip("Name of the stage select scene to load.")]
    public string stageSelectSceneName = "StageSelect";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void TryTravelToPlanet()
    {
        if (selectedPlanetName == "???")
        {
            if (sfxSource != null && errorSound != null)
            {
                sfxSource.PlayOneShot(errorSound);
            }
            else
            {
                Debug.LogWarning("SFX Source or Error Sound not assigned in PlanetToStageManager.");
            }

            Debug.Log("Travel denied: planet is unknown (???).");
            return;
        }
        if (!Application.CanStreamedLevelBeLoaded(stageSelectSceneName))
        {
            Debug.LogError("Scene not found: " + stageSelectSceneName);
            return;
        }
        SceneManager.LoadScene(stageSelectSceneName);
    }
}
