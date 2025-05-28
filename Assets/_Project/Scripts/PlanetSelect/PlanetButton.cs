using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Represents an individual planet button in the UI. 
/// When clicked, it notifies the PlanetSelector to update the selected planet and play the spin animation.
/// </summary>
public class PlanetButton : MonoBehaviour
{
    [Tooltip("The display name of the planet (e.g., 'Holy Planet', 'Rock').")]
    public string planetName;

    [Tooltip("Reference to the planet's Animator component for triggering spin animation.")]
    public Animator planetAnimator;

    private PlanetSelector planetSelector;

    /// <summary>
    /// Caches the PlanetSelector reference when this button is initialized.
    /// </summary>
    private void Awake()
    {
        // ✅ Replace deprecated method with FindFirstObjectByType
        planetSelector = Object.FindFirstObjectByType<PlanetSelector>();

        if (planetSelector == null)
        {
            Debug.LogError("PlanetSelector not found in scene.");
        }
    }

    /// <summary>
    /// Called by the UI Button OnClick() event.
    /// Passes this planet's name and Animator to the PlanetSelector.
    /// </summary>
    public void OnClick()
    {
        if (planetSelector != null)
        {
            planetSelector.SelectPlanet(planetName, planetAnimator);
        }
        else
        {
            Debug.LogWarning("PlanetSelector not available when trying to select planet: " + planetName);
        }
    }
}
