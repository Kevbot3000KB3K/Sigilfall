using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the Sigil Library UI. Handles tab switching, slot initialization, selection feedback, and sigil display.
/// </summary>
public class LibraryTabController : MonoBehaviour
{
    public static LibraryTabController Instance;

    [Header("Slots Per Element")]
    public SigilSlotUI[] arcaneSlots, fireSlots, iceSlots, windSlots, woodSlots, rockSlots,
                         waterSlots, ghostSlots, primalSlots, holySlots, lightningSlots, darkSlots;

    [Header("Tabs")]
    public LibraryTab[] allTabs;

    private LibraryTab currentTab;

    [Header("Selection Visual")]
    public GameObject selectionPrefab;
    private GameObject activeSelectionHighlight;

    [Header("Fallback Sprite")]
    public Sprite unknownSigilSprite;

    [HideInInspector] public SigilSlotUI currentSelectedSlot;

    void Awake() => Instance = this;

    void Start() => PopulateAllTabs();

    /// <summary>
    /// Opens a specific tab, hides others, and plays SFX.
    /// </summary>
    public void OpenTab(LibraryTab tabToOpen)
    {
        SLSoundFX.Instance?.PlaySFX(SLSoundFX.Instance.pageTurnSFX);
        tabToOpen.transform.SetAsLastSibling();

        foreach (var tab in allTabs)
            tab.slotsContainer.SetActive(tab == tabToOpen);

        currentTab = tabToOpen;
    }

    /// <summary>
    /// Highlights and selects the clicked slot. Updates selection visual.
    /// </summary>
    public void SelectSlot(SigilSlotUI newSlot)
    {
        currentSelectedSlot?.SetHighlight(false);
        currentSelectedSlot = newSlot;
        newSlot.SetHighlight(true);

        SLSoundFX.Instance?.PlaySFX(SLSoundFX.Instance.slotSelect);

        if (selectionPrefab != null)
        {
            if (activeSelectionHighlight == null)
                activeSelectionHighlight = Instantiate(selectionPrefab, newSlot.transform);

            activeSelectionHighlight.transform.SetParent(newSlot.transform);
            activeSelectionHighlight.transform.localPosition = new Vector3(252f, 12f, 0f);
            activeSelectionHighlight.transform.localScale = Vector3.one;
            activeSelectionHighlight.transform.SetAsLastSibling();
        }

        string nameToShow = newSlot.sigilData != null ? newSlot.sigilData.sigilName : "???";
        Debug.Log($"✅ Selected Sigil: {nameToShow}");
    }

    /// <summary>
    /// Returns the corresponding slot array for a given Family.
    /// </summary>
    public SigilSlotUI[] GetSlotArrayForFamily(Family family)
    {
        var db = FamilyDatabaseLoader.Instance;

        if (family == db.arcane) return arcaneSlots;
        if (family == db.fire) return fireSlots;
        if (family == db.ice) return iceSlots;
        if (family == db.wind) return windSlots;
        if (family == db.wood) return woodSlots;
        if (family == db.rock) return rockSlots;
        if (family == db.water) return waterSlots;
        if (family == db.ghost) return ghostSlots;
        if (family == db.primal) return primalSlots;
        if (family == db.holy) return holySlots;
        if (family == db.lightning) return lightningSlots;
        if (family == db.dark) return darkSlots;

        return null;
    }

    /// <summary>
    /// Repopulates the currently opened tab's slots.
    /// </summary>
    public void RefreshCurrentTab()
    {
        if (currentTab == null) return;
        PopulateFamilySlots(currentTab.family, GetSlotArrayForFamily(currentTab.family));
    }

    /// <summary>
    /// Fills all slots for the specified family.
    /// </summary>
    public void PopulateFamilySlots(Family family, SigilSlotUI[] slots)
    {
        var sigils = SigilDatabase.Instance.GetSigilsByFamily(family);

        for (int i = 0; i < slots.Length; i++)
        {
            if (i >= sigils.Count)
            {
                slots[i].gameObject.SetActive(false);
                continue;
            }

            var sigil = sigils[i];
            bool discovered = PlayerSigilCollection.Instance.HasDiscovered(sigil);

            slots[i].gameObject.SetActive(true);
            slots[i].Initialize(sigil, discovered, this);
        }
    }

    /// <summary>
    /// Populates all tabs for every family.
    /// </summary>
    public void PopulateAllTabs()
    {
        var db = FamilyDatabaseLoader.Instance;
        PopulateFamilySlots(db.arcane, arcaneSlots);
        PopulateFamilySlots(db.fire, fireSlots);
        PopulateFamilySlots(db.holy, holySlots);
        PopulateFamilySlots(db.dark, darkSlots);
        PopulateFamilySlots(db.rock, rockSlots);
        PopulateFamilySlots(db.lightning, lightningSlots);
        PopulateFamilySlots(db.water, waterSlots);
        PopulateFamilySlots(db.ice, iceSlots);
        PopulateFamilySlots(db.wind, windSlots);
        PopulateFamilySlots(db.primal, primalSlots);
        PopulateFamilySlots(db.ghost, ghostSlots);
        PopulateFamilySlots(db.wood, woodSlots);
    }

    /// <summary>
    /// Updates all currently visible slots after discovery changes.
    /// </summary>
    public void UpdateDiscoveredSlots() => RefreshCurrentTab();

    /// <summary>
    /// Debug-only: unlocks all sigils and repopulates all tabs.
    /// </summary>
    public void DebugPopulateAll()
    {
        PlayerSigilCollection.Instance.DiscoverAll();
        PopulateAllTabs();
    }

    /// <summary>
    /// Equips the currently selected sigil (must be discovered).
    /// </summary>
    public void EquipSigil()
    {
        PlayerData.Instance.EquipSigil(currentSelectedSlot.sigilData);
        SLSoundFX.Instance?.PlaySFX(SLSoundFX.Instance.equipSigil);
    }
}
