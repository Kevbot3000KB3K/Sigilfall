using UnityEngine;

/// <summary>
/// A Sigil is a crafted magical entity made from three Shards.
/// It defines the ball's appearance, behavior, elemental family, and gameplay effect.
/// </summary>
[CreateAssetMenu(fileName = "New Sigil", menuName = "Sigils/Sigil")]
public class Sigil : ScriptableObject
{
    [Header("Core Info")]
    [Tooltip("The display name of this Sigil.")]
    public string sigilName;

    [Tooltip("The visual sprite used to represent the Sigil.")]
    public Sprite sigilSprite;

    [Tooltip("Unique identifier string (for sorting, saving, or cross-referencing).")]
    public string IDNumber;

    [Tooltip("The color used for the ball's trail renderer.")]
    public Color trailColor = Color.white;

    [TextArea]
    [Tooltip("Optional description of the Sigil's lore or effect.")]
    public string description;

    [Header("Elemental Attributes")]
    [Tooltip("The elemental family this Sigil belongs to (e.g., Fire, Ice).")]
    public Family family;

    [Tooltip("A family this Sigil has an advantage over (for elemental matchups).")]
    public Family advantage;

    [Header("Gameplay Properties")]
    [Tooltip("The difficulty rating of this Sigil from 1 (easy) to 5 (hard).")]
    [Range(1, 5)]
    public int difficulty;

    [Tooltip("Special effect this Sigil applies when used as a ball.")]
    public BallEffect special;

    [Header("Crafting Ingredients")]
    [Tooltip("The first Shard used to craft this Sigil.")]
    public Shard ingredient1;

    [Tooltip("The second Shard used to craft this Sigil.")]
    public Shard ingredient2;

    [Tooltip("The third Shard used to craft this Sigil.")]
    public Shard ingredient3;

    /// <summary>
    /// Gets the base speed of the ball when this Sigil is used, based on its difficulty.
    /// </summary>
    public int BallSpeed
    {
        get
        {
            return difficulty switch
            {
                1 => 10,
                2 => 15,
                3 => 20,
                4 => 25,
                5 => 30,
                _ => 10
            };
        }
    }
}
