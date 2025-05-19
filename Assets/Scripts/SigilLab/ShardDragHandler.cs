using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShardDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Shard shardData; // ScriptableObject reference
    public Image icon;
    public ShardSlotUI originSlotUI; // The source UI for refunding
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private GameObject draggingIcon;
    private Vector3 originalPosition;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        // 🔧 Try to assign icon sprite if missing
        Image img = GetComponent<Image>();
        if (img != null && shardData != null)
        {
            img.sprite = shardData.icon;
            img.preserveAspect = true;
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalPosition = transform.position;

        draggingIcon = Instantiate(gameObject, canvas.transform);
        draggingIcon.transform.SetAsLastSibling();

        CanvasGroup group = draggingIcon.GetComponent<CanvasGroup>();
        if (group == null) group = draggingIcon.AddComponent<CanvasGroup>();
        group.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (draggingIcon != null)
        {
            draggingIcon.transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

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
            transform.position = originalPosition;
            transform.SetParent(originalParent);
            Debug.Log("Shard was not placed in any alter.");
        }

        Destroy(draggingIcon);
    }
}
