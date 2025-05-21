using UnityEngine;
using UnityEngine.UI;

public class SigilHUDDisplay : MonoBehaviour
{
    [Header("UI References")]
    public Image equippedSigilImage;
    public Image specialEffectImage;

    void Start()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (PlayerData.Instance != null && PlayerData.Instance.HasEquippedSigil())
        {
            Sigil sigil = PlayerData.Instance.equippedSigil;

            if (equippedSigilImage != null)
                equippedSigilImage.sprite = sigil.sigilSprite;

            if (specialEffectImage != null && sigil.special != null)
                specialEffectImage.sprite = sigil.special.effectIcon;
        }
        else
        {
            Debug.LogWarning("No sigil equipped or PlayerData not found.");
        }
    }
}
