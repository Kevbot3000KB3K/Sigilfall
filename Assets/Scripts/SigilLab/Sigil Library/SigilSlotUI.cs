using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Represents a single UI slot in the Sigil Library. 
/// Displays discovered/undiscovered sigils and handles selection/highlight state.
/// </summary>
public class SigilSlotUI : MonoBehaviour
{
    [Header("Auto-Assigned UI References")]
    public Image sigilIcon;                  // Icon showing the sigil sprite or unknown placeholder
    public TextMeshProUGUI sigilNameText;    // Text showing the sigil's name or "???"
    public GameObject highlightVisual;       // Optional highlight overlay object

    [HideInInspector] public Sigil sigilData; // Stored reference to the assigned sigil
    private LibraryTabController libraryController; // Back-reference to the controller managing this slot

    /// <summary>
    /// Auto-assigns missing references for icon and name UI on awake.
    /// </summary>
    private void Awake()
    {
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
    }

    /// <summary>
    /// Sets up this UI slot with the given sigil data, controller, and discovered status.
    /// </summary>
    /// <param name="sigil">The sigil this slot represents.</param>
    /// <param name="discovered">Whether this sigil has been discovered by the player.</param>
    /// <param name="controller">The controller managing this UI slot.</param>
    public void Initialize(Sigil sigil, bool discovered, LibraryTabController controller)
    {
        sigilData = sigil;
        libraryController = controller;

        // Set visuals based on discovery state
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

        // Assign click handler to button if present
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

    /// <summary>
    /// Called when this slot is clicked. Informs the library controller.
    /// </summary>
    public void OnClick()
    {
        if (libraryController == null)
        {
            Debug.LogError($"❌ Slot {transform.GetSiblingIndex()} was clicked but not initialized!");
            return;
        }

        libraryController.SelectSlot(this);
    }

    /// <summary>
    /// Toggles the visual highlight effect on or off.
    /// </summary>
    /// <param name="on">True to show highlight, false to hide it.</param>
    public void SetHighlight(bool on)
    {
        if (highlightVisual != null)
            highlightVisual.SetActive(on);
    }
}
