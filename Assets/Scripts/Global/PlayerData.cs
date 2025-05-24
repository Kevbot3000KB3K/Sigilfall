using UnityEngine;

/// <summary>
/// Holds persistent player-related data across scenes, such as the currently equipped Sigil.
/// Implements a Singleton pattern.
/// </summary>
public class PlayerData : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the PlayerData accessible globally.
    /// </summary>
    public static PlayerData Instance;

    /// <summary>
    /// The currently equipped Sigil by the player.
    /// </summary>
    public Sigil equippedSigil;

    /// <summary>
    /// Initializes the singleton instance and prevents duplicate copies.
    /// Persists across scene loads.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Sets the equipped sigil and logs it for debug purposes.
    /// </summary>
    /// <param name="newSigil">The new Sigil to equip.</param>
    public void EquipSigil(Sigil newSigil)
    {
        equippedSigil = newSigil;
        Debug.Log($"🎯 Equipped Sigil: {newSigil.sigilName}");
    }

    /// <summary>
    /// Checks if the player has an equipped Sigil.
    /// </summary>
    /// <returns>True if a sigil is equipped; otherwise, false.</returns>
    public bool HasEquippedSigil() => equippedSigil != null;
}
