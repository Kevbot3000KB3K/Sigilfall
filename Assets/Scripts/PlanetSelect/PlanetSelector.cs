using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles UI logic for planet selection, including animation triggers, audio feedback,
/// and scene transitions based on selected planet.
/// </summary>
public class PlanetSelector : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Text element that displays the name of the currently selected planet.")]
    public TextMeshProUGUI planetNameText;

    [Tooltip("Highlight objects used to visually indicate selected planets.")]
    public GameObject[] planetHighlightImages;

    [Header("Audio")]
    private PlanetSelectAudio audioManager;

    private Animator lastSelectedAnimator;
    private string currentPlanetName;

    // Optional: Keep for future use (e.g., locking buttons)
    private Button lastSelectedPlanetButton;

    /// <summary>
    /// Maps planet display names to their corresponding scene names.
    /// </summary>
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

    /// <summary>
    /// Initializes the audio manager when the scene starts.
    /// </summary>
    private void Start()
    {
        // ✅ Use modern Unity API
        audioManager = Object.FindFirstObjectByType<PlanetSelectAudio>();

        if (audioManager == null)
        {
            Debug.LogWarning("PlanetSelectAudio not found in scene.");
        }
    }

    /// <summary>
    /// Called when a planet button is clicked. Updates UI and audio.
    /// </summary>
    /// <param name="planetName">The name of the selected planet.</param>
    public void OnPlanetButtonClicked(string planetName)
    {
        currentPlanetName = planetName;
        PlanetToStageManager.Instance.selectedPlanetName = planetName;
        planetNameText.text = planetName;

        audioManager?.PlaySelectionSound();
    }

    /// <summary>
    /// Selects a planet, plays its spin animation, and stops the previous one.
    /// </summary>
    /// <param name="planetName">Name of the planet to select.</param>
    /// <param name="planetAnimator">Animator associated with the selected planet.</param>
    public void SelectPlanet(string planetName, Animator planetAnimator)
    {
        currentPlanetName = planetName;
        PlanetToStageManager.Instance.selectedPlanetName = planetName;
        planetNameText.text = planetName;

        audioManager?.PlaySelectionSound();

        // Stop animation of previously selected planet
        if (lastSelectedAnimator != null && lastSelectedAnimator != planetAnimator)
        {
            lastSelectedAnimator.ResetTrigger("Selected");
            lastSelectedAnimator.SetTrigger("Idle");
        }

        // Play selected animation on current planet
        if (planetAnimator != null)
        {
            planetAnimator.ResetTrigger("Idle");
            planetAnimator.SetTrigger("Selected");
            lastSelectedAnimator = planetAnimator;
        }
    }

    /// <summary>
    /// Called when the "Travel" button is clicked. Loads the scene associated with the selected planet.
    /// </summary>
    public void OnTravelButtonClicked()
    {
        if (currentPlanetName == "???")
        {
            audioManager?.PlayErrorSound();
            return;
        }

        audioManager?.PlayConfirmSound();

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
