using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance;

    public Sigil equippedSigil; // ← The currently equipped sigil

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

    public void EquipSigil(Sigil newSigil)
    {
        equippedSigil = newSigil;
        Debug.Log($"🎯 Equipped Sigil: {newSigil.sigilName}");
    }

    public bool HasEquippedSigil() => equippedSigil != null;
}
