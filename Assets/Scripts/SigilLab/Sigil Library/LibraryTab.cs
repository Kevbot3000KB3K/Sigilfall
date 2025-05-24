using UnityEngine;

/// <summary>
/// Represents a single tab in the Sigil Library UI.
/// Each tab is linked to a specific Family (element) and contains a container for its sigil slots.
/// </summary>
public class LibraryTab : MonoBehaviour
{
    [Header("Tab Configuration")]

    [Tooltip("The element family this tab represents.")]
    public Family family;

    [Tooltip("The container GameObject that holds the sigil slots for this family.")]
    public GameObject slotsContainer;
}
