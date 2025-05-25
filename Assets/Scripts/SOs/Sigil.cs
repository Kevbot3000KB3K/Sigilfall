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

    [Header("Advantage Modifiers")]
    [Tooltip("Damage modifier vs Holy bricks (✨).")]
    public int vsHoly;

    [Tooltip("Damage modifier vs Dark bricks (🌙).")]
    public int vsDark;

    [Tooltip("Damage modifier vs Rock bricks (🗻).")]
    public int vsRock;

    [Tooltip("Damage modifier vs Wind bricks (💨).")]
    public int vsWind;

    [Tooltip("Damage modifier vs Fire bricks (🔥).")]
    public int vsFire;

    [Tooltip("Damage modifier vs Water bricks (💧).")]
    public int vsWater;

    [Tooltip("Damage modifier vs Lightning bricks (⚡).")]
    public int vsLightning;

    [Tooltip("Damage modifier vs Ice bricks (🧊).")]
    public int vsIce;

    [Tooltip("Damage modifier vs Wood bricks (🌳).")]
    public int vsWood;

    [Tooltip("Damage modifier vs Ghost bricks (👻).")]
    public int vsGhost;

    [Tooltip("Damage modifier vs Primal bricks (🐾).")]
    public int vsPrimal;

    [Tooltip("Damage modifier vs Arcane bricks (🌠).")]
    public int vsArcane;

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
    public int GetModifierAgainst(Family targetFamily)
    {
        return targetFamily.name switch
        {
            "Holy" => vsHoly,
            "Dark" => vsDark,
            "Rock" => vsRock,
            "Wind" => vsWind,
            "Fire" => vsFire,
            "Water" => vsWater,
            "Lightning" => vsLightning,
            "Ice" => vsIce,
            "Wood" => vsWood,
            "Ghost" => vsGhost,
            "Primal" => vsPrimal,
            "Arcane" => vsArcane,
            _ => 1 // default minimum damage
        };
    }

}
