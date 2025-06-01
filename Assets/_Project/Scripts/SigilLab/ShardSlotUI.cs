using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections.Generic;

/// <summary>
/// Handles UI behavior for a single shard slot in the Sigil Lab.
/// Allows dragging shards into Alter slots and updates count and visuals.
/// </summary>
public class ShardSlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Shard Data")]
    public Shard shardData;
    public int count = 99;

    [Header("UI References")]
    public Image icon;
    public TextMeshProUGUI counterText;
    public GameObject shardDragPrefab; // Prefab for dragging shard visuals

    private GameObject draggingShardImage;
    private Canvas parentCanvas;

    /// <summary>
    /// Initializes references and refreshes UI visuals.
    /// </summary>
    void Start()
    {
        parentCanvas = GetComponentInParent<Canvas>();
        RefreshUI();
    }

    /// <summary>
    /// Updates the icon and counter text to reflect current shard data and count.
    /// </summary>
    public void RefreshUI()
    {
        icon.sprite = shardData.icon;
        counterText.text = count.ToString();
    }

    /// <summary>
    /// Adds one shard back to this slot and refreshes UI.
    /// Called when a shard is refunded.
    /// </summary>
    public void RefundShard()
    {
        count++;
        RefreshUI();
    }

    /// <summary>
    /// Called when drag begins. Spawns a draggable image of the shard.
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (count <= 0) return;

        SLSoundFX.Instance?.PlaySFX(SLSoundFX.Instance.shardPickUpSFX);

        draggingShardImage = Instantiate(shardDragPrefab, parentCanvas.transform);
        draggingShardImage.transform.SetAsLastSibling();
        draggingShardImage.transform.localScale = Vector3.one * 2f;

        RectTransform rt = draggingShardImage.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(64, 64);

        ShardDragHandler handler = draggingShardImage.GetComponent<ShardDragHandler>();
        handler.shardData = shardData;
        handler.originSlotUI = this;

        Image img = draggingShardImage.GetComponent<Image>();
        if (img != null)
        {
            img.sprite = shardData.icon;
            img.preserveAspect = true;
            img.raycastTarget = false;
        }

        UpdateDraggingPosition(eventData);
    }

    /// <summary>
    /// Called while dragging. Updates the dragging image's position.
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        UpdateDraggingPosition(eventData);
    }

    /// <summary>
    /// Updates the dragging shard image's position to follow the mouse.
    /// </summary>
    void UpdateDraggingPosition(PointerEventData eventData)
    {
        if (draggingShardImage != null)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentCanvas.transform as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPoint
            );

            draggingShardImage.transform.localPosition = localPoint;
        }
    }

    /// <summary>
    /// Called when dragging ends. Attempts to place the shard in a valid AlterSlot.
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        if (draggingShardImage != null)
        {
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            Debug.Log($"Raycast hit count: {results.Count}");

            bool placed = false;

            foreach (var result in results)
            {
                GameObject current = result.gameObject;
                AlterSlot alterSlot = current.GetComponent<AlterSlot>();

                while (alterSlot == null && current.transform.parent != null)
                {
                    current = current.transform.parent.gameObject;
                    alterSlot = current.GetComponent<AlterSlot>();
                }

                if (alterSlot != null && alterSlot.currentShard == null)
                {
                    Debug.Log("✅ AlterSlot found and is empty!");

                    alterSlot.SetShard(shardData, this);
                    count--;
                    RefreshUI();
                    placed = true;
                    break;
                }
            }

            Destroy(draggingShardImage);

            if (!placed)
            {
                Debug.Log("❌ Shard was not placed in any alter.");
            }
        }
    }
}
