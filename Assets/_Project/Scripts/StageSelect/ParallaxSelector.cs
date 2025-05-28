using UnityEngine;

/// <summary>
/// Handles switching between parallax background sets for different stage selections.
/// Only one parallax layer is active at a time.
/// </summary>
public class ParallaxSelector : MonoBehaviour
{
    [Header("Parallax Backgrounds")]
    [Tooltip("Parallax GameObject for Stage I.")]
    public GameObject stageIParallax;

    [Tooltip("Parallax GameObject for Stage II.")]
    public GameObject stageIIParallax;

    [Tooltip("Parallax GameObject for Stage III.")]
    public GameObject stageIIIParallax;

    /// <summary>
    /// Activates the parallax background for Stage I.
    /// </summary>
    public void ShowStageI()
    {
        ActivateParallax(stageIParallax);
    }

    /// <summary>
    /// Activates the parallax background for Stage II.
    /// </summary>
    public void ShowStageII()
    {
        ActivateParallax(stageIIParallax);
    }

    /// <summary>
    /// Activates the parallax background for Stage III.
    /// </summary>
    public void ShowStageIII()
    {
        ActivateParallax(stageIIIParallax);
    }

    /// <summary>
    /// Disables all parallax backgrounds, then enables the one passed in.
    /// </summary>
    /// <param name="target">The parallax GameObject to activate.</param>
    private void ActivateParallax(GameObject target)
    {
        // Deactivate all
        if (stageIParallax != null) stageIParallax.SetActive(false);
        if (stageIIParallax != null) stageIIParallax.SetActive(false);
        if (stageIIIParallax != null) stageIIIParallax.SetActive(false);

        // Activate target if not null
        if (target != null)
            target.SetActive(true);
    }
}
