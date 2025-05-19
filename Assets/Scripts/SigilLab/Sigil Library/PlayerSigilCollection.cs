using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class PlayerSigilCollection : MonoBehaviour
{
    public static PlayerSigilCollection Instance;

    private HashSet<Sigil> discoveredSigils = new HashSet<Sigil>();

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

    public void Discover(Sigil sigil)
    {
        if (sigil != null && !discoveredSigils.Contains(sigil))
        {
            discoveredSigils.Add(sigil);
            Debug.Log($"📘 Discovered new Sigil: {sigil.sigilName}");
        }
    }

    public bool HasDiscovered(Sigil sigil)
    {
        return discoveredSigils.Contains(sigil);
    }

    public void DiscoverAll()
    {
        discoveredSigils = new HashSet<Sigil>(SigilDatabase.Instance.allSigils);
    }

    public List<Sigil> GetDiscovered()
    {
        return discoveredSigils.ToList();
    }
}
