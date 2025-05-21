using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LibraryTabController : MonoBehaviour
{
    public static LibraryTabController Instance;

    [Header("Slots Per Element")]
    public SigilSlotUI[] arcaneSlots;
    public SigilSlotUI[] fireSlots;
    public SigilSlotUI[] iceSlots;
    public SigilSlotUI[] windSlots;
    public SigilSlotUI[] woodSlots;
    public SigilSlotUI[] rockSlots;
    public SigilSlotUI[] waterSlots;
    public SigilSlotUI[] ghostSlots;
    public SigilSlotUI[] primalSlots;
    public SigilSlotUI[] holySlots;
    public SigilSlotUI[] lightningSlots;
    public SigilSlotUI[] darkSlots;

    public LibraryTab[] allTabs; // Assign all 12 tabs in the Inspector

    private LibraryTab currentTab;

    [Header("Selection Visual")]
    public GameObject selectionPrefab; // ← assign your LibrarySelection prefab
    private GameObject activeSelectionHighlight; // currently spawned instance


    public Sprite unknownSigilSprite;
    [HideInInspector] public SigilSlotUI currentSelectedSlot;
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        PopulateAllTabs(); // or similar method that fills all tabs
    }

    public void OpenTab(LibraryTab tabToOpen)
    {
        // 🔊 Play page turn sound
        SLSoundFX.Instance?.PlaySFX(SLSoundFX.Instance.pageTurnSFX);

        // Bring selected tab to front
        tabToOpen.transform.SetAsLastSibling();

        // Show this tab's slots, hide others
        foreach (var tab in allTabs)
            tab.slotsContainer.SetActive(tab == tabToOpen);

        currentTab = tabToOpen;
    }
    public void SelectSlot(SigilSlotUI newSlot)
    {
        // Unhighlight old slot
        if (currentSelectedSlot != null)
            currentSelectedSlot.SetHighlight(false);

        currentSelectedSlot = newSlot;
        currentSelectedSlot.SetHighlight(true);
        SLSoundFX.Instance?.PlaySFX(SLSoundFX.Instance.slotSelect);

        // Handle selection visual
        if (selectionPrefab != null)
        {
            if (activeSelectionHighlight == null)
            {
                activeSelectionHighlight = Instantiate(selectionPrefab, currentSelectedSlot.transform);
            }

            activeSelectionHighlight.transform.SetParent(currentSelectedSlot.transform);
            activeSelectionHighlight.transform.localPosition = new Vector3(252f, 12f, 0f);
            activeSelectionHighlight.transform.localScale = Vector3.one;
            activeSelectionHighlight.transform.SetAsLastSibling();
        }

        // ✅ Add this to safely log the name (even if sigil is undiscovered)
        string nameToShow = newSlot.sigilData != null ? newSlot.sigilData.sigilName : "???";
        Debug.Log($"✅ Selected Sigil: {nameToShow}");
    }
    public SigilSlotUI[] GetSlotArrayForFamily(Family family)
    {
        if (family == FamilyDatabaseLoader.Instance.arcane) return arcaneSlots;
        if (family == FamilyDatabaseLoader.Instance.fire) return fireSlots;
        if (family == FamilyDatabaseLoader.Instance.ice) return iceSlots;
        if (family == FamilyDatabaseLoader.Instance.wind) return windSlots;
        if (family == FamilyDatabaseLoader.Instance.wood) return woodSlots;
        if (family == FamilyDatabaseLoader.Instance.rock) return rockSlots;
        if (family == FamilyDatabaseLoader.Instance.water) return waterSlots;
        if (family == FamilyDatabaseLoader.Instance.ghost) return ghostSlots;
        if (family == FamilyDatabaseLoader.Instance.primal) return primalSlots;
        if (family == FamilyDatabaseLoader.Instance.holy) return holySlots;
        if (family == FamilyDatabaseLoader.Instance.lightning) return lightningSlots;
        if (family == FamilyDatabaseLoader.Instance.dark) return darkSlots;

        return null;
    }

    public void RefreshCurrentTab()
    {
        if (currentTab != null)
        {
            Family currentFamily = currentTab.family;
            SigilSlotUI[] slots = GetSlotArrayForFamily(currentFamily);

            PopulateFamilySlots(currentFamily, slots);
        }
    }


    public void PopulateFamilySlots(Family family, SigilSlotUI[] slots)
    {
        List<Sigil> familySigils = SigilDatabase.Instance.GetSigilsByFamily(family);

        for (int i = 0; i < slots.Length; i++)
        {
            if (i >= familySigils.Count)
            {
                slots[i].gameObject.SetActive(false);
                continue;
            }

            Sigil sigil = familySigils[i];
            bool discovered = PlayerSigilCollection.Instance.HasDiscovered(sigil);

            slots[i].gameObject.SetActive(true);
            Debug.Log($"📘 Initializing Slot for {sigil.sigilName} - Discovered: {discovered}");
            slots[i].Initialize(sigil, discovered, this);
        }
    }
    public void PopulateAllTabs()
    {
        PopulateFamilySlots(FamilyDatabaseLoader.Instance.arcane, arcaneSlots);
        PopulateFamilySlots(FamilyDatabaseLoader.Instance.fire, fireSlots);
        PopulateFamilySlots(FamilyDatabaseLoader.Instance.holy, holySlots);
        PopulateFamilySlots(FamilyDatabaseLoader.Instance.dark, darkSlots);
        PopulateFamilySlots(FamilyDatabaseLoader.Instance.rock, rockSlots);
        PopulateFamilySlots(FamilyDatabaseLoader.Instance.lightning, lightningSlots);
        PopulateFamilySlots(FamilyDatabaseLoader.Instance.water, waterSlots);
        PopulateFamilySlots(FamilyDatabaseLoader.Instance.ice, iceSlots);
        PopulateFamilySlots(FamilyDatabaseLoader.Instance.wind, windSlots);
        PopulateFamilySlots(FamilyDatabaseLoader.Instance.primal, primalSlots);
        PopulateFamilySlots(FamilyDatabaseLoader.Instance.ghost, ghostSlots);
        PopulateFamilySlots(FamilyDatabaseLoader.Instance.wood, woodSlots);
    }



    public void UpdateDiscoveredSlots()
    {
        if (currentTab != null)
        {
            Family family = currentTab.family;
            SigilSlotUI[] slots = GetSlotArrayForFamily(family);
            PopulateFamilySlots(family, slots);
        }
    }

    public void DebugPopulateAll()
    {
        PlayerSigilCollection.Instance.DiscoverAll(); // unlock all sigils (for testing)

        PopulateFamilySlots(FamilyDatabaseLoader.Instance.arcane, arcaneSlots);
        PopulateFamilySlots(FamilyDatabaseLoader.Instance.fire, fireSlots);
        PopulateFamilySlots(FamilyDatabaseLoader.Instance.ice, iceSlots);
        PopulateFamilySlots(FamilyDatabaseLoader.Instance.wind, windSlots);
        PopulateFamilySlots(FamilyDatabaseLoader.Instance.wood, woodSlots);
        PopulateFamilySlots(FamilyDatabaseLoader.Instance.rock, rockSlots);
        PopulateFamilySlots(FamilyDatabaseLoader.Instance.water, waterSlots);
        PopulateFamilySlots(FamilyDatabaseLoader.Instance.ghost, ghostSlots);
        PopulateFamilySlots(FamilyDatabaseLoader.Instance.primal, primalSlots);
        PopulateFamilySlots(FamilyDatabaseLoader.Instance.holy, holySlots);
        PopulateFamilySlots(FamilyDatabaseLoader.Instance.lightning, lightningSlots);
        PopulateFamilySlots(FamilyDatabaseLoader.Instance.dark, darkSlots);
    }

    public void EquipSigil()
    {
        PlayerData.Instance.EquipSigil(LibraryTabController.Instance.currentSelectedSlot.sigilData);

        SLSoundFX.Instance?.PlaySFX(SLSoundFX.Instance.equipSigil);
    }


}
