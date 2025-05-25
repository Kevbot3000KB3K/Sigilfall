using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class SigilImporter : EditorWindow
{
    private const string csvFilePath = "Assets/Editor/SigilElementalModifiers.csv";
    private const string sigilFolderPath = "Resources/Sigils";

    [MenuItem("Tools/Import Sigil Modifiers")]
    public static void ShowWindow()
    {
        GetWindow<SigilImporter>("Sigil Importer").Show();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Import Sigil Damage Modifiers from CSV"))
        {
            ImportCSV();
        }
    }

    private static void ImportCSV()
    {
        if (!File.Exists(csvFilePath))
        {
            Debug.LogError("CSV file not found at: " + csvFilePath);
            return;
        }

        string[] lines = File.ReadAllLines(csvFilePath);
        string[] headers = lines[0].Split(',');

        for (int i = 1; i < lines.Length; i++)
        {
            string[] cols = lines[i].Split(',');
            string id = "#" + int.Parse(cols[0]).ToString("D3");


            // Load Sigil asset by IDNumber
            Sigil[] sigils = Resources.LoadAll<Sigil>("Sigils");
            Sigil target = sigils.FirstOrDefault(s => s.IDNumber == id);

            if (target == null)
            {
                Debug.LogWarning($"⚠️ Could not find Sigil with ID #{id}");
                continue;
            }

            target.vsHoly = int.Parse(cols[1]);
            target.vsDark = int.Parse(cols[2]);
            target.vsRock = int.Parse(cols[3]);
            target.vsWind = int.Parse(cols[4]);
            target.vsFire = int.Parse(cols[5]);
            target.vsWater = int.Parse(cols[6]);
            target.vsLightning = int.Parse(cols[7]);
            target.vsIce = int.Parse(cols[8]);
            target.vsWood = int.Parse(cols[9]);
            target.vsGhost = int.Parse(cols[10]);
            target.vsPrimal = int.Parse(cols[11]);
            target.vsArcane = int.Parse(cols[12]);

            EditorUtility.SetDirty(target);
        }

        AssetDatabase.SaveAssets();
        Debug.Log("✅ Sigil modifiers imported successfully.");
    }
}
