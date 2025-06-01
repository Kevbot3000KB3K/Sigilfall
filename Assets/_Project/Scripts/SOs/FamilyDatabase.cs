using UnityEngine;

/// <summary>
/// A centralized database holding references to all Family ScriptableObjects.
/// Use this to access families consistently across systems (UI, game logic, etc.).
/// </summary>
[CreateAssetMenu(fileName = "FamilyDatabase", menuName = "Databases/FamilyDatabase")]
public class FamilyDatabase : ScriptableObject
{
    [Header("Elemental Families")]

    [Tooltip("Arcane-type family (magic, mana, etc).")]
    public Family arcane;

    [Tooltip("Fire-type family (flame, heat, combustion).")]
    public Family fire;

    [Tooltip("Ice-type family (frost, snow, chilling).")]
    public Family ice;

    [Tooltip("Wind-type family (air, gust, motion).")]
    public Family wind;

    [Tooltip("Wood-type family (nature, vines, growth).")]
    public Family wood;

    [Tooltip("Rock-type family (earth, stone, defense).")]
    public Family rock;

    [Tooltip("Water-type family (fluidity, waves, flow).")]
    public Family water;

    [Tooltip("Ghost-type family (spiritual, ethereal, haunting).")]
    public Family ghost;

    [Tooltip("Primal-type family (beasts, instincts, wild magic).")]
    public Family primal;

    [Tooltip("Holy-type family (light, sanctity, order).")]
    public Family holy;

    [Tooltip("Lightning-type family (electricity, storm, charge).")]
    public Family lightning;

    [Tooltip("Dark-type family (shadow, corruption, void).")]
    public Family dark;
}
