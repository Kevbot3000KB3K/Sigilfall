using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Singleton class that holds and manages access to all discovered Sigils.
/// Loads them from the Resources folder at runtime.
/// </summary>
public class SigilDatabase : MonoBehaviour
{
    public static SigilDatabase Instance;

    [Tooltip("All sigils currently available in the game. Automatically loaded from Resources/Sigils.")]
    public List<Sigil> allSigils = new List<Sigil>();

    /// <summary>
    /// Initializes the Singleton instance and loads all Sigil ScriptableObjects from Resources.
    /// </summary>
    private void Awake()
    {
        Instance = this;

        // Load all Sigils from the "Resources/Sigils" folder
        allSigils = Resources.LoadAll<Sigil>("Sigils").ToList();

        Debug.Log($"✅ Loaded {allSigils.Count} sigils from Resources.");
    }

    /// <summary>
    /// Finds a Sigil whose recipe matches the given combination of 3 shards.
    /// The order of shards does not matter.
    /// </summary>
    /// <param name="ingredients">Array of 3 Shard ScriptableObjects.</param>
    /// <returns>The matching Sigil if found; otherwise, null.</returns>
    public Sigil GetSigilFromShards(Shard[] ingredients)
    {
        var sortedInput = ingredients.OrderBy(s => s.name).ToArray();

        foreach (Sigil sigil in allSigils)
        {
            var recipe = new[] { sigil.ingredient1, sigil.ingredient2, sigil.ingredient3 }
                         .OrderBy(s => s.name).ToArray();

            if (sortedInput.SequenceEqual(recipe))
                return sigil;
        }

        return null;
    }

    /// <summary>
    /// Returns the full list of all loaded Sigils.
    /// </summary>
    public List<Sigil> GetAllSigils()
    {
        return allSigils;
    }

    /// <summary>
    /// Returns all Sigils that belong to a specific Family.
    /// </summary>
    /// <param name="family">The family to filter by.</param>
    /// <returns>A list of Sigils from the given family.</returns>
    public List<Sigil> GetSigilsByFamily(Family family)
    {
        return allSigils.Where(s => s.family == family).ToList();
    }
}
