using UnityEngine;

[CreateAssetMenu(menuName = "Sigils/Shard")]
public class Shard : ScriptableObject
{
    public string shardName;
    public Sprite icon;
    public Color shardColor; // ✅ Add this

    // You can also store the hex for reference (optional)
    [HideInInspector] public string hexValue;
}
