using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class SigilSlotUI : MonoBehaviour
{
    [Header("Auto-Assigned UI References")]
    public Image sigilIcon;
    public TextMeshProUGUI sigilNameText;
    public GameObject highlightVisual;

    [HideInInspector] public Sigil sigilData;
    private LibraryTabController libraryController;

    private void Awake()
    {
        // Keep auto-assigns if needed
        if (sigilIcon == null)
        {
            Transform spriteChild = transform.Find("Sigil Sprite");
            if (spriteChild != null)
                sigilIcon = spriteChild.GetComponent<Image>();
        }

        if (sigilNameText == null)
        {
            Transform nameChild = transform.Find("Sigil Name");
            if (nameChild != null)
                sigilNameText = nameChild.GetComponent<TextMeshProUGUI>();
        }

        // DO NOT assign button in Awake anymore
    }


    public void Initialize(Sigil sigil, bool discovered, LibraryTabController controller)
    {
        sigilData = sigil;
        libraryController = controller;

        if (discovered && sigil != null)
        {
            sigilIcon.sprite = sigil.sigilSprite;
            sigilNameText.text = sigil.sigilName;
        }
        else
        {
            sigilIcon.sprite = controller.unknownSigilSprite;
            sigilNameText.text = "???";
        }

        SetHighlight(false);
        Transform buttonArea = transform.Find("Button Area");
        if (buttonArea != null)
        {
            Button button = buttonArea.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(OnClick);
            }
        }
        else
        {
            Debug.LogWarning($"⚠️ Could not find 'Button Area' under {name}");
        }
    }


    public void OnClick()
    {
        if (libraryController == null)
        {
            Debug.LogError($"❌ Slot {transform.GetSiblingIndex()} was clicked but not initialized!");
            return;
        }

        libraryController.SelectSlot(this);
    }





    public void SetHighlight(bool on)
    {
        if (highlightVisual != null)
            highlightVisual.SetActive(on);
    }
}
