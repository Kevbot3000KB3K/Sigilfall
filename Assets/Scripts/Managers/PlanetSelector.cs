using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PlanetSelector : MonoBehaviour
{
    private Dictionary<string, string> planetToSceneMap = new Dictionary<string, string>()
    {
        { "Holy Planet", "StageSelect_Holy" },
        { "Dark", "StageSelect_Dark" },
        { "Rock", "StageSelect_Rock" },
        { "Wind", "StageSelect_Wind" },
        { "Fir", "StageSelect_Fire" },
        { "Water", "StageSelect_Water" },
        { "Lightning", "StageSelect_Lightning" },
        { "Wood", "StageSelect_Wood" },
        { "Arcane", "StageSelect_Arcane" },
        { "Ice", "StageSelect_Ice" },
        { "Primal", "StageSelect_Primal" },
        { "Ghost", "StageSelect_Ghost" },
    };

    private PlanetSelectAudio audioManager;

    public TextMeshProUGUI planetNameText;
    private Button lastSelectedPlanetButton;
    public GameObject[] planetHighlightImages;

    private string currentPlanetName;

    void Start()
    {
        audioManager = FindObjectOfType<PlanetSelectAudio>();
    }

    public void OnPlanetButtonClicked(string planetName)
    {
        currentPlanetName = planetName;
        PlanetToStageManager.Instance.selectedPlanetName = planetName;
        planetNameText.text = planetName;
        if (audioManager != null)
            audioManager.PlaySelectionSound();
    }

    public void OnTravelButtonClicked()
    {
        if (currentPlanetName == "???")
        {
            if (audioManager != null)
            {
                audioManager.PlayErrorSound();
            }
            return;
        }
        if (audioManager != null)
        {
            audioManager.PlayConfirmSound();
        }
        if (!string.IsNullOrEmpty(currentPlanetName))
        {
            if (planetToSceneMap.TryGetValue(currentPlanetName, out string sceneName))
            {
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                Debug.LogError("Scene not found for planet: " + currentPlanetName);
            }
        }
        else
        {
            Debug.LogWarning("No planet selected!");
        }
    }
}
