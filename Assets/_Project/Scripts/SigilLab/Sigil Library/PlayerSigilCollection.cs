using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Manages which sigils the player has discovered during gameplay.
/// Implements Singleton pattern and persists across scenes.
/// </summary>
public class PlayerSigilCollection : MonoBehaviour
{
    public static PlayerSigilCollection Instance;

    // Stores discovered sigils
    private HashSet<Sigil> discoveredSigils = new HashSet<Sigil>();

    /// <summary>
    /// Initializes the singleton instance and ensures persistence across scenes.
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
            Destroy(gameObject); // Prevent duplicates
        }
    }

    /// <summary>
    /// Marks a sigil as discovered if it hasn't already been discovered.
    /// </summary>
    /// <param name="sigil">The sigil to mark as discovered.</param>
    public void Discover(Sigil sigil)
    {
        if (sigil != null && discoveredSigils.Add(sigil))
        {
            Debug.Log($"📘 Discovered new Sigil: {sigil.sigilName}");
        }
    }

    /// <summary>
    /// Checks if a specific sigil has been discovered.
    /// </summary>
    /// <param name="sigil">The sigil to check.</param>
    /// <returns>True if discovered, false otherwise.</returns>
    public bool HasDiscovered(Sigil sigil)
    {
        return discoveredSigils.Contains(sigil);
    }

    /// <summary>
    /// Discovers all sigils from the database (useful for testing/debugging).
    /// </summary>
    public void DiscoverAll()
    {
        discoveredSigils = new HashSet<Sigil>(SigilDatabase.Instance.allSigils);
    }

    /// <summary>
    /// Returns a list of all currently discovered sigils.
    /// </summary>
    public List<Sigil> GetDiscovered()
    {
        return discoveredSigils.ToList();
    }
}
