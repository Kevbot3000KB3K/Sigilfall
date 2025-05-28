using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Represents a slot in the Sigil Lab where shards can be placed.
/// Manages visual feedback, shard assignment, and communication with SigilLabManager.
/// </summary>
public class AlterSlot : MonoBehaviour
{
    [Header("Slot Visuals")]
    [SerializeField] private Image slotImage;
    [SerializeField] private Sprite emptySlotSprite;
    public Light2D alterLight;

    [Header("Shard Visual Prefab")]
    public GameObject shardUIPrefab;

    /// <summary>Reference to the currently placed shard's drag handler (not used in placement).</summary>
    public ShardDragHandler currentShard { get; private set; }

    /// <summary>Reference to the currently placed shard ScriptableObject.</summary>
    public Shard currentShardSO { get; private set; }

    /// <summary>Returns true if this slot currently has a shard placed in it.</summary>
    public bool IsOccupied => currentShardSO != null;

    private GameObject visualShardObj;         // Instance of the shard UI visual
    private ShardSlotUI sourceSlotUI;          // Origin slot for refunding when cleared

    /// <summary>
    /// Called when a draggable shard is dropped into this slot.
    /// Stores its data and triggers visual/audio feedback.
    /// </summary>
    public bool AcceptShard(ShardDragHandler shard)
    {
        SetShard(shard.shardData, shard.originSlotUI);
        return true;
    }

    /// <summary>
    /// Assigns a shard to this slot. Replaces any existing shard and spawns visual feedback.
    /// </summary>
    /// <param name="shardSO">The shard ScriptableObject to assign.</param>
    /// <param name="originSlot">The inventory slot it came from (used for refund).</param>
    public void SetShard(Shard shardSO, ShardSlotUI originSlot)
    {
        Debug.Log($"SetShard called with SO: {shardSO.name}");

        // Refund previous shard if one already existed
        if (IsOccupied && sourceSlotUI != null)
        {
            sourceSlotUI.RefundShard();

            if (visualShardObj != null)
                Destroy(visualShardObj);
        }

        // Store new shard and origin
        currentShardSO = shardSO;
        sourceSlotUI = originSlot;

        // Update lighting based on shard color
        if (alterLight != null)
            alterLight.color = shardSO.shardColor;

        // Play shard drop SFX
        SLSoundFX.Instance?.PlaySFX(SLSoundFX.Instance.shardDropSFX);

        // Instantiate visual shard prefab
        if (shardUIPrefab != null)
        {
            visualShardObj = Instantiate(shardUIPrefab, transform);
            visualShardObj.transform.localPosition = Vector3.zero;

            Image img = visualShardObj.GetComponent<Image>();
            if (img != null)
            {
                img.sprite = shardSO.icon;
                img.preserveAspect = true;
            }
        }

        // Check if all alters are filled (to enable combine button)
        SigilLabManager.Instance?.CheckForFullAlters();
    }

    /// <summary>
    /// Clears the shard from this slot and resets visuals and references.
    /// </summary>
    public void Clear()
    {
        // Destroy visual shard image
        if (visualShardObj != null)
        {
            Destroy(visualShardObj);
            visualShardObj = null;
        }

        currentShard = null;
        currentShardSO = null;
        sourceSlotUI = null;

        // Reset the slot image to empty
        if (slotImage != null)
        {
            slotImage.sprite = emptySlotSprite;
            slotImage.enabled = true;
        }

        // Reset the light to neutral
        if (alterLight != null)
            alterLight.color = Color.white;
    }
}
