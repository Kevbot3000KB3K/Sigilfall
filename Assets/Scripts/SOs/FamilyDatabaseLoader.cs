using UnityEngine;

/// <summary>
/// Provides global access to the FamilyDatabase ScriptableObject using a lazy-loaded singleton pattern.
/// Loads the database from the "Resources" folder. Ensure a file named "FamilyDatabase.asset" exists there.
/// </summary>
public static class FamilyDatabaseLoader
{
    /// <summary>
    /// Cached reference to the loaded FamilyDatabase instance.
    /// </summary>
    private static FamilyDatabase _instance;

    /// <summary>
    /// Gets the singleton instance of the FamilyDatabase.
    /// Loads it from the Resources folder if it hasn't already been loaded.
    /// </summary>
    public static FamilyDatabase Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<FamilyDatabase>("FamilyDatabase");

                if (_instance == null)
                {
                    Debug.LogError("❌ FamilyDatabase asset not found in Resources folder! Please place the asset at Resources/FamilyDatabase.");
                }
            }

            return _instance;
        }
    }
}
