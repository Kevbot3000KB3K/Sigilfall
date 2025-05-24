using UnityEngine;

/// <summary>
/// Represents a basic elemental shard used to craft Sigils. 
/// Each shard has a name, icon, and color, and can optionally include its hex value for UI or serialization.
/// </summary>
[CreateAssetMenu(menuName = "Sigils/Shard")]
public class Shard : ScriptableObject
{
    [Tooltip("Display name of the shard (e.g., Ember, Frostbite, Echo).")]
    public string shardName;

    [Tooltip("Visual icon representing this shard.")]
    public Sprite icon;

    [Tooltip("Base color of the shard, used for visual effects or UI accenting.")]
    public Color shardColor;

    [Tooltip("Hexadecimal representation of the shard color (auto-populated or reference-only).")]
    [HideInInspector]
    public string hexValue; // Optional: can be used for web UI or export data
}
