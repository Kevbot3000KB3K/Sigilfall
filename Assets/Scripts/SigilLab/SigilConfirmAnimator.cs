using UnityEngine;
using UnityEngine.UI;

public class SigilConfirmAnimator : MonoBehaviour
{
    [Header("Sigil Movement")]
    public RectTransform sigilImage;
    public RectTransform targetLibraryIcon;
    public GameObject sparkleEffectPrefab;
    public float moveDuration = 0.6f;
    public float shrinkScale = 0.1f;

    [Header("Lab Reference")]
    public SigilLabManager labManager;

    public void AnimateConfirmAndReset()
    {

        Vector3 startPos = sigilImage.position;
        Vector3 endPos = targetLibraryIcon.position;

        Vector3 startScale = sigilImage.localScale;
        Vector3 endScale = startScale * shrinkScale; // ✅ Just shrink from current size

        // Move + shrink
        LeanTween.move(sigilImage.gameObject, endPos, moveDuration).setEaseInOutQuad();
        LeanTween.value(sigilImage.gameObject, startScale, endScale, moveDuration)
            .setOnUpdate((Vector3 val) => sigilImage.localScale = val)
            .setEaseInOutQuad()
            .setOnComplete(() =>
            {
                sigilImage.gameObject.SetActive(false);

                if (sparkleEffectPrefab != null)
                {
                    GameObject sparkle = Instantiate(sparkleEffectPrefab, endPos, Quaternion.identity, targetLibraryIcon.parent);
                    Destroy(sparkle, 1f);
                }

                labManager.ResetSigilLab();
                sigilImage.localScale = startScale;
                sigilImage.anchoredPosition = Vector2.zero;
            });
    }


}
