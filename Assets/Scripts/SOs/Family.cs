using UnityEngine;

/// <summary>
/// Represents an elemental or thematic family that a Sigil belongs to (e.g., Fire, Water, Ghost).
/// Used to categorize and filter Sigils by type.
/// </summary>
[CreateAssetMenu(fileName = "New Family", menuName = "Sigils/Family")]
public class Family : ScriptableObject
{
    [Tooltip("The display name of the family (e.g., Fire, Ice, Ghost).")]
    public string familyName;

    [Tooltip("Icon used to visually represent this family in UI.")]
    public Sprite familyIcon;
}
