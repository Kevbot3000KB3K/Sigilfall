using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SigilDatabase : MonoBehaviour
{
    public static SigilDatabase Instance;

    public List<Sigil> allSigils = new List<Sigil>();


    private void Awake()
    {
        Instance = this;
        allSigils = Resources.LoadAll<Sigil>("Sigils").ToList();


        Debug.Log("Loaded " + allSigils.Count + " sigils...");

    }

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

    public List<Sigil> GetAllSigils()
    {
        return allSigils;
    }

    public List<Sigil> GetSigilsByFamily(Family family)
    {
        return allSigils.Where(s => s.family == family).ToList();
    }


}
