using UnityEngine;

public static class FamilyDatabaseLoader
{
    private static FamilyDatabase _instance;

    public static FamilyDatabase Instance
    {
        get
        {
            if (_instance == null)
                _instance = Resources.Load<FamilyDatabase>("FamilyDatabase");

            if (_instance == null)
                Debug.LogError("❌ FamilyDatabase asset not found in Resources folder!");

            return _instance;
        }
    }
}
