using UnityEngine;

public class ParallaxSelector : MonoBehaviour
{
    public GameObject stageIParallax;
    public GameObject stageIIParallax;
    public GameObject stageIIIParallax;

    public void ShowStageI()
    {
        ActivateParallax(stageIParallax);
    }

    public void ShowStageII()
    {
        ActivateParallax(stageIIParallax);
    }

    public void ShowStageIII()
    {
        ActivateParallax(stageIIIParallax);
    }

    private void ActivateParallax(GameObject target)
    {
        stageIParallax.SetActive(false);
        stageIIParallax.SetActive(false);
        stageIIIParallax.SetActive(false);
        if (target != null)
            target.SetActive(true);
    }
}
