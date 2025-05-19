using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ShardSlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Shard shardData;
    public Image icon;
    public TextMeshProUGUI counterText;
    public int count = 99;
    public GameObject shardDragPrefab; // ✅ Assign this in Inspector

    private GameObject draggingShardImage;
    private Canvas parentCanvas;

    void Start()
    {
        parentCanvas = GetComponentInParent<Canvas>();
        RefreshUI();
    }

    public void RefreshUI()
    {
        icon.sprite = shardData.icon;
        counterText.text = count.ToString();
    }

    public void RefundShard()
    {
        count++;
        RefreshUI();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (count <= 0) return;

        SLSoundFX.Instance?.PlaySFX(SLSoundFX.Instance.shardPickUpSFX);

        // ✅ Instantiate dragging shard prefab
        draggingShardImage = Instantiate(shardDragPrefab, parentCanvas.transform);
        draggingShardImage.transform.SetAsLastSibling();

        // ✅ Set position and scale
        RectTransform rt = draggingShardImage.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(64, 64); // Adjust if needed

        // ✅ Set shard data
        ShardDragHandler handler = draggingShardImage.GetComponent<ShardDragHandler>();
        handler.shardData = shardData;
        handler.originSlotUI = this;

        // ✅ Update the Image component with the shard sprite
        Image img = draggingShardImage.GetComponent<Image>();
        if (img != null)
        {
            img.sprite = shardData.icon;           // 🎯 Set correct sprite
            img.preserveAspect = true;
            img.raycastTarget = false;
        }
        draggingShardImage.transform.localScale = Vector3.one * 2f;
        UpdateDraggingPosition(eventData);
    }


    public void OnDrag(PointerEventData eventData)
    {
        UpdateDraggingPosition(eventData);
    }

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
                Debug.Log($"→ Hit: {result.gameObject.name}");

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
