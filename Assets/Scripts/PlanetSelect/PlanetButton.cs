using UnityEngine;
using UnityEngine.UI;

public class PlanetButton : MonoBehaviour
{
    public string planetName;
    public Animator planetAnimator;

    public void OnClick()
    {
        // Pass planet name and animator to selector
        FindObjectOfType<PlanetSelector>().SelectPlanet(planetName, planetAnimator);
    }
}
