using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Handles the drag-and-drop behavior for shard icons.
/// Allows shards to be picked up from inventory and dropped into AlterSlots.
/// </summary>
public class ShardDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Tooltip("The shard ScriptableObject data associated with this UI element.")]
    public Shard shardData;

    [Tooltip("Reference to the image representing the shard.")]
    public Image icon;

    [Tooltip("Reference to the originating UI slot (for potential refunds).")]
    public ShardSlotUI originSlotUI;

    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private Vector3 originalPosition;
    private GameObject draggingIcon;

    /// <summary>
    /// Initializes references and ensures the shard icon is properly displayed.
    /// </summary>
    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        // Automatically assign icon sprite if available
        if (icon != null && shardData != null)
        {
            icon.sprite = shardData.icon;
            icon.preserveAspect = true;
        }
    }

    /// <summary>
    /// Called when drag begins. Clones the shard icon and prepares it for dragging.
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalPosition = transform.position;

        draggingIcon = Instantiate(gameObject, canvas.transform);
        draggingIcon.transform.SetAsLastSibling();

        var group = draggingIcon.GetComponent<CanvasGroup>();
        if (group == null) group = draggingIcon.AddComponent<CanvasGroup>();
        group.blocksRaycasts = false; // Needed to raycast into drop targets
    }

    /// <summary>
    /// Updates the position of the dragging icon to follow the mouse.
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        if (draggingIcon != null)
        {
            draggingIcon.transform.position = Input.mousePosition;
        }
    }

    /// <summary>
    /// Called when the drag ends. Attempts to place the shard into an AlterSlot.
    /// If placement fails, returns to original position.
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        bool placed = false;

        foreach (var result in results)
        {
            GameObject current = result.gameObject;
            AlterSlot alterSlot = current.GetComponent<AlterSlot>();

            // Traverse up the hierarchy to find a valid AlterSlot
            while (alterSlot == null && current.transform.parent != null)
            {
                current = current.transform.parent.gameObject;
                alterSlot = current.GetComponent<AlterSlot>();
            }

            if (alterSlot != null && alterSlot.AcceptShard(this))
            {
                transform.SetParent(alterSlot.transform);
                transform.position = alterSlot.transform.position;
                placed = true;
                break;
            }
        }

        if (!placed)
        {
            // Return to original location
            transform.position = originalPosition;
            transform.SetParent(originalParent);
            Debug.Log("❌ Shard was not placed in any alter.");
        }

        // Clean up visual clone
        Destroy(draggingIcon);
    }
}
