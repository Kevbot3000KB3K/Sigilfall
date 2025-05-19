using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

public class AlterSlot : MonoBehaviour
{
    public ShardDragHandler currentShard { get; private set; }
    public Shard currentShardSO { get; private set; }
    public GameObject shardUIPrefab;
    [SerializeField] private Sprite emptySlotSprite;

    [SerializeField] private Image slotImage;
    public Light2D alterLight;
    public bool IsOccupied => currentShardSO != null;

    private GameObject visualShardObj;
    private ShardSlotUI sourceSlotUI;

    public bool AcceptShard(ShardDragHandler shard)
    {
        SetShard(shard.shardData, shard.originSlotUI); // passing shard's SO and source
        return true;
    }

    public void SetShard(Shard shardSO, ShardSlotUI originSlot)
    {
        Debug.Log($"SetShard called with SO: {shardSO.name}");

        // If a shard is already in this slot, destroy it and refund to previous source
        if (IsOccupied && sourceSlotUI != null)
        {
            sourceSlotUI.RefundShard();
            if (visualShardObj != null)
                Destroy(visualShardObj);
        }

        currentShardSO = shardSO;
        sourceSlotUI = originSlot;
        if (alterLight != null)
            alterLight.color = shardSO.shardColor;
        // 🔊 Play sound
        SLSoundFX.Instance?.PlaySFX(SLSoundFX.Instance.shardDropSFX);

        // Spawn visual
        if (shardUIPrefab != null)
        {
            visualShardObj = Instantiate(shardUIPrefab, this.transform);
            visualShardObj.transform.localPosition = Vector3.zero;

            Image img = visualShardObj.GetComponent<Image>();
            if (img != null)
            {
                img.sprite = shardSO.icon;
                img.preserveAspect = true;
            }
        }
        SigilLabManager.Instance?.CheckForFullAlters();
    }

    public void Clear()
    {
        if (visualShardObj != null)
        {
            Destroy(visualShardObj);
            visualShardObj = null;
        }

        currentShard = null;
        currentShardSO = null;
        sourceSlotUI = null;

        if (slotImage != null)
        {
            slotImage.sprite = emptySlotSprite; // ← Reset to placeholder
            slotImage.enabled = true;
        }

        if (alterLight != null)
            alterLight.color = Color.white; // or your default neutral light


    }
}
