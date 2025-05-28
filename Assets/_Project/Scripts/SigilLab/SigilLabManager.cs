using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Controls the behavior of the Sigil Lab, including shard combination, sigil reveal, UI animations, and equipping.
/// </summary>
public class SigilLabManager : MonoBehaviour
{
    public static SigilLabManager Instance;

    [Header("Lab UI References")]
    public AlterSlot[] alterSlots;
    public Button combineButton;
    public Button confirmButton;
    public GameObject sigilDisplayArea;
    public Image sigilDisplayImage;
    public Transform alembic, spinningRing, shardSlotGroup;
    public Animator shimmerAnim;
    public GameObject shimmerOverlay;
    public Image whiteFlashImage;

    [Header("Sigil Info UI")]
    public TextMeshProUGUI sigilNameText, idNumberText, descriptionText;
    public Image familyIcon, specialEffectIcon;
    public GameObject[] difficultyStars;
    public Image[] ingredientIcons;
    public GameObject infoUIPanel;
    public Image newNote;

    [Header("Reveal Panels")]
    public RectTransform namePanel, descriptionPanel, difficultyPanel, familyPanel;
    public RectTransform ingredientsPanel, idNumberPanel, specialPanel, newNotePanel;

    [Header("Advantages Panel")]
    public GameObject advantagesPanel;
    public Image holyAdv, darkAdv, rockAdv, windAdv, fireAdv, waterAdv, lightningAdv, iceAdv, woodAdv, ghostAdv, primalAdv, arcaneAdv;
    [Tooltip("Sprites representing modifier strength (index 0 = 1Mod, index 4 = 5Mod).")]
    public Sprite[] modSprites; // Should be size 5
    [Header("Advantage Mod Display")]
    public Sprite modEmpty; // The fully empty sprite (no nodes lit)

    [Header("Advantage Mod Sounds")]
    public AudioClip[] modSounds; // 5 clips from Mod1 to Mod5 (index = modifier - 1)
    public AudioSource advantageAudioSource; // Use dedicated source for clean layering

    [Header("Library UI")]
    public RectTransform libraryPanel, libraryIconTarget;
    public float libraryTweenDuration = 0.8f;
    [Header("Duplicate Warning UI")]
    public GameObject duplicateWarningPanel;
    public TextMeshProUGUI duplicateWarningText;
    public AudioClip errorSFX;
    public AudioSource uiAudioSource;
    private CanvasGroup duplicateWarningGroup;


    private Coroutine shimmerLoopRoutine;
    private bool isInformReveal = false;
    private float alembicStartY, ringStartY, shardSlotsStartY;

    void Awake() => Instance = this;

    void Start()
    {
        alembicStartY = alembic.localPosition.y;
        ringStartY = spinningRing.localPosition.y;
        shardSlotsStartY = shardSlotGroup.localPosition.y;
        combineButton.gameObject.SetActive(false);
        if (duplicateWarningPanel != null)
            duplicateWarningGroup = duplicateWarningPanel.GetComponent<CanvasGroup>();
    }

    /// <summary>
    /// Checks if all alter slots are filled and toggles the combine button accordingly.
    /// </summary>
    public void CheckForFullAlters()
    {
        combineButton.gameObject.SetActive(alterSlots.All(slot => slot.currentShardSO != null));
    }

    /// <summary>
    /// Triggers shard combination and begins sigil reveal animation.
    /// </summary>
    public void CombineShards()
    {
        Sigil result = SigilDatabase.Instance.GetSigilFromShards(alterSlots.Select(s => s.currentShardSO).ToArray());

        if (result == null)
        {
            Debug.LogWarning("❌ No sigil result for this combination.");
            return;
        }

        if (PlayerSigilCollection.Instance.HasDiscovered(result))
        {
            Debug.Log($"🛑 Already discovered: {result.sigilName}");

            // Play error sound
            if (uiAudioSource != null && errorSFX != null)
                uiAudioSource.PlayOneShot(errorSFX);

            // Show popup warning
            if (duplicateWarningPanel != null && duplicateWarningText != null)
            {
                duplicateWarningText.text = $"You’ve already discovered the <b><color=#00FFFF>{result.sigilName} Sigil</color></b>!";

                duplicateWarningPanel.SetActive(true);

                // Optional auto-hide
                StartCoroutine(HideDuplicateWarningAfterDelay(2.5f));
            }

            return; // Exit early
        }

        // Proceed with combination animation
        SLSoundFX.Instance?.PlaySFX(SLSoundFX.Instance.sigilCombine);
        combineButton.gameObject.SetActive(false);

        float targetY = 425f;
        float duration = 1.2f;
        int finishedTweens = 0;

        System.Action onComplete = () => { if (++finishedTweens >= 3) ShowSigil(result); };

        LeanTween.moveLocalY(alembic.gameObject, targetY, duration).setEaseOutCubic().setOnComplete(onComplete);
        LeanTween.moveLocalY(spinningRing.gameObject, targetY, duration).setEaseOutCubic().setOnComplete(onComplete);
        LeanTween.moveLocalY(shardSlotGroup.gameObject, targetY, duration).setEaseOutCubic().setOnComplete(onComplete);
    }


    /// <summary>
    /// Animates a UI panel from its current to target position.
    /// </summary>
    void AnimatePanelIn(RectTransform panel, Vector2 targetPos, float delay = 0f)
    {
        panel.gameObject.SetActive(true); // ensure it's visible

        Vector2 startPos = panel.anchoredPosition;

        LeanTween.value(panel.gameObject, startPos, targetPos, 0.8f)
            .setEase(LeanTweenType.easeOutCubic)
            .setDelay(delay)
            .setOnUpdate((Vector2 val) => {
                panel.anchoredPosition = val;
            })
            .setOnComplete(() => {
                panel.anchoredPosition = targetPos;
            });
    }


    /// <summary>
    /// Displays a sigil and animates associated UI elements.
    /// </summary>
    void ShowSigil(Sigil sigil)
    {
        if (sigil == null) { Debug.Log("❌ No sigil was created."); return; }

        if (!isInformReveal)
        {
            PlayerSigilCollection.Instance.Discover(sigil);
            var family = sigil.family;

            if (LibraryTabController.Instance == null)
            {
                Debug.LogError("❌ LibraryTabController.Instance is null!");
            }
            else if (family == null)
            {
                Debug.LogError($"❌ Sigil '{sigil.sigilName}' has a null family!");
            }
            else if (LibraryTabController.Instance.GetSlotArrayForFamily(family) == null)
            {
                Debug.LogError($"❌ No slot array found for family {family.name}.");
            }
            else
            {
                LibraryTabController.Instance.PopulateFamilySlots(family, LibraryTabController.Instance.GetSlotArrayForFamily(family));
            }
        }

        else newNotePanel.gameObject.SetActive(false);

        ResetRevealPanelPositions();

        sigilDisplayArea.SetActive(true);
        infoUIPanel.SetActive(true);
        // Clear all before animation starts
        Image[] advImages = {
        holyAdv, darkAdv, rockAdv, windAdv, fireAdv, waterAdv,
        lightningAdv, iceAdv, woodAdv, ghostAdv, primalAdv, arcaneAdv
            };


        foreach (var img in advImages)
        {
            img.sprite = modEmpty;
            img.preserveAspect = true;
        }
        sigilDisplayImage.rectTransform.localScale = Vector3.one;
        sigilDisplayImage.rectTransform.anchoredPosition = Vector2.zero;
        sigilDisplayImage.transform.localPosition = Vector3.zero;
        StartCoroutine(AnimateSigilReveal(sigil));

        AnimatePanelIn(namePanel, new Vector2(-3.85f, 4f));
        AnimatePanelIn(descriptionPanel, new Vector2(-5f, -3.3f), 0.1f);
        AnimatePanelIn(difficultyPanel, new Vector2(-4.5f, 2.8f), 0.2f);
        AnimatePanelIn(familyPanel, new Vector2(-7.5f, 2.5f), 0.25f);
        AnimatePanelIn(ingredientsPanel, new Vector2(-7.48f, 1.814f), 0.3f);
        AnimatePanelIn(idNumberPanel, new Vector2(1.9f, 4f), 0.35f);
        AnimatePanelIn(specialPanel, new Vector2(-6f, -0.25f), 0.4f);
        AnimatePanelIn(advantagesPanel.GetComponent<RectTransform>(), new Vector2(7.25f, 0.9f), 0.4f);
        if (!isInformReveal) AnimatePanelIn(newNotePanel, new Vector2(4f, 4f), 0.5f);
        StartCoroutine(DelayedAdvantageAnimation(sigil, 1.25f));


        sigilNameText.text = $"{sigil.sigilName} Sigil";
        familyIcon.sprite = sigil.family.familyIcon;
        specialEffectIcon.sprite = sigil.special.effectIcon;
        idNumberText.text = $"{sigil.IDNumber:D3}";
        descriptionText.text = sigil.description;

        ingredientIcons[0].sprite = sigil.ingredient1.icon;
        ingredientIcons[1].sprite = sigil.ingredient2.icon;
        ingredientIcons[2].sprite = sigil.ingredient3.icon;
        foreach (var icon in ingredientIcons) icon.preserveAspect = true;

        for (int i = 0; i < difficultyStars.Length; i++)
            difficultyStars[i].SetActive(i < sigil.difficulty);

        shimmerOverlay.SetActive(true);
        shimmerAnim?.Play("Shimmer");
        if (!isInformReveal) SLSoundFX.Instance?.PlaySFX(SLSoundFX.Instance.sigilRevealSFX);
        sigilDisplayImage.gameObject.SetActive(true);

        confirmButton.gameObject.SetActive(true);
        if (shimmerLoopRoutine != null) StopCoroutine(shimmerLoopRoutine);
        shimmerLoopRoutine = StartCoroutine(PlayShimmerLoop());
        isInformReveal = false;

        advantagesPanel.SetActive(true);
    }

    /// <summary>
    /// Tween in the library UI from the left.
    /// </summary>
    public void ShowLibrary()
    {
        libraryPanel.gameObject.SetActive(true);
        libraryPanel.anchoredPosition = new Vector2(-1920f, 0f);
        LeanTween.moveLocalX(libraryPanel.gameObject, 0f, libraryTweenDuration).setEaseOutCubic();
        SLSoundFX.Instance?.PlaySFX(SLSoundFX.Instance.pageTurnSFX);
    }

    /// <summary>
    /// Tween out the library UI to the left.
    /// </summary>
    public void HideLibrary()
    {
        LeanTween.moveLocalX(libraryPanel.gameObject, -1920f, libraryTweenDuration).setEaseInCubic()
                 .setOnComplete(() => libraryPanel.gameObject.SetActive(false));
        SLSoundFX.Instance?.PlaySFX(SLSoundFX.Instance.pageTurnSFX);
    }

    /// <summary>
    /// Reset the lab to its initial state after revealing a sigil.
    /// </summary>
    public void ResetSigilLab()
    {
        float duration = 1.2f;
        LeanTween.moveLocalY(alembic.gameObject, alembicStartY, duration).setEaseInCubic();
        LeanTween.moveLocalY(spinningRing.gameObject, ringStartY, duration).setEaseInCubic();
        LeanTween.moveLocalY(shardSlotGroup.gameObject, shardSlotsStartY, duration).setEaseInCubic();

        sigilDisplayArea.SetActive(false);
        infoUIPanel.SetActive(false);
        shimmerOverlay.SetActive(false);
        confirmButton.gameObject.SetActive(false);
        sigilDisplayImage.gameObject.SetActive(true);
        sigilDisplayImage.rectTransform.localScale = Vector3.one;
        sigilDisplayImage.rectTransform.anchoredPosition = Vector2.zero;
        sigilDisplayImage.color = new Color(1f, 1f, 1f, 0f); // Transparent, ready for next fade-in

        foreach (var slot in alterSlots) slot.Clear();
        ResetRevealPanelPositions();
        SLSoundFX.Instance?.PlaySFX(SLSoundFX.Instance.pageTurnSFX);
    }

    /// <summary>
    /// Reset panel positions before revealing a sigil.
    /// </summary>
    private void ResetRevealPanelPositions()
    {
        namePanel.anchoredPosition = new Vector2(-1300f, 4f);
        descriptionPanel.anchoredPosition = new Vector2(-1300f, -3.3f);
        difficultyPanel.anchoredPosition = new Vector2(-1300f, 2.8f);
        familyPanel.anchoredPosition = new Vector2(-1300f, 2.5f);
        ingredientsPanel.anchoredPosition = new Vector2(1300f, 1.2f);
        idNumberPanel.anchoredPosition = new Vector2(1300f, 3.8f);
        specialPanel.anchoredPosition = new Vector2(-1300f, 0.4f);
        newNotePanel.anchoredPosition = new Vector2(16f, 4f);
        advantagesPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(1300f, 0.9f);


        if (shimmerLoopRoutine != null)
        {
            StopCoroutine(shimmerLoopRoutine);
            shimmerLoopRoutine = null;
        }
        shimmerOverlay.SetActive(false);
    }

    /// <summary>
    /// Coroutine for animated reveal of the sigil sprite with flash effect.
    /// </summary>
    private IEnumerator AnimateSigilReveal(Sigil sigil)
    {
        sigilDisplayImage.sprite = sigil.sigilSprite;
        sigilDisplayImage.color = new Color(1, 1, 1, 0f);
        sigilDisplayImage.gameObject.SetActive(true);
        whiteFlashImage.color = new Color(1, 1, 1, 0f);
        whiteFlashImage.gameObject.SetActive(true);

        LeanTween.value(whiteFlashImage.gameObject, 0f, 1f, 0.3f).setOnUpdate(val => whiteFlashImage.color = new Color(1, 1, 1, val));
        yield return new WaitForSeconds(0.3f);

        LeanTween.value(sigilDisplayImage.gameObject, 0f, 1f, 0.3f).setOnUpdate(val => sigilDisplayImage.color = new Color(1, 1, 1, val));
        yield return new WaitForSeconds(0.3f);

        LeanTween.value(whiteFlashImage.gameObject, 1f, 0f, 0.4f).setOnUpdate(val => whiteFlashImage.color = new Color(1, 1, 1, val));
        yield return new WaitForSeconds(0.4f);

        whiteFlashImage.gameObject.SetActive(false);
    }

    /// <summary>
    /// Loop shimmer animation during sigil display.
    /// </summary>
    private IEnumerator PlayShimmerLoop()
    {
        while (true)
        {
            shimmerOverlay.SetActive(true);
            shimmerAnim.Play("Shimmer", -1, 0f);
            yield return new WaitForSeconds(shimmerAnim.GetCurrentAnimatorStateInfo(0).length);
            shimmerOverlay.SetActive(false);
            yield return new WaitForSeconds(3f);
        }
    }

    /// <summary>
    /// Triggers reveal animation from library selection.
    /// </summary>
    public void RevealSigilFromLibrary()
    {
        var slot = LibraryTabController.Instance.currentSelectedSlot;
        if (slot == null || slot.sigilData == null || !PlayerSigilCollection.Instance.HasDiscovered(slot.sigilData))
        {
            Debug.LogWarning("⚠️ Cannot reveal unknown or unselected sigil.");
            SLSoundFX.Instance?.PlaySFX(SLSoundFX.Instance.error);
            return;
        }

        HideLibrary();
        float targetY = 425f;
        float duration = 1.2f;
        int finishedTweens = 0;

        System.Action onComplete = () => { if (++finishedTweens >= 3) { isInformReveal = true; ShowSigil(slot.sigilData); } };

        LeanTween.moveLocalY(alembic.gameObject, targetY, duration).setEaseOutCubic().setOnComplete(onComplete);
        LeanTween.moveLocalY(spinningRing.gameObject, targetY, duration).setEaseOutCubic().setOnComplete(onComplete);
        LeanTween.moveLocalY(shardSlotGroup.gameObject, targetY, duration).setEaseOutCubic().setOnComplete(onComplete);

        foreach (var slotUI in alterSlots) slotUI.Clear();
        combineButton.gameObject.SetActive(false);
    }

    /// <summary>
    /// Sets the player's equipped sigil from the current library slot.
    /// </summary>
    public void EquipSigil()
    {
        var selected = LibraryTabController.Instance.currentSelectedSlot?.sigilData;
        if (selected != null)
        {
            PlayerData.Instance.EquipSigil(selected);
            SLSoundFX.Instance?.PlaySFX(SLSoundFX.Instance.equipSigil);
        }
        else
        {
            Debug.LogWarning("⚠️ No sigil selected to equip.");
            SLSoundFX.Instance?.PlaySFX(SLSoundFX.Instance.error);
        }
    }
    private void SetAdvantageDisplay(Image targetImage, int modifier)
    {
        if (modifier < 1 || modifier > 5)
        {
            Debug.LogWarning("Modifier out of range: " + modifier);
            return;
        }

        if (modSprites.Length < 5)
        {
            Debug.LogWarning("Mod Sprites not fully assigned!");
            return;
        }

        targetImage.sprite = modSprites[modifier - 1];
        targetImage.preserveAspect = true;
    }
    private IEnumerator AnimateAdvantagePanel(Sigil sigil)
    {
        var delay = new WaitForSeconds(0.04f);

        List<(Image image, int value)> entries = new()
    {
        (holyAdv, sigil.vsHoly),
        (darkAdv, sigil.vsDark),
        (rockAdv, sigil.vsRock),
        (windAdv, sigil.vsWind),
        (fireAdv, sigil.vsFire),
        (waterAdv, sigil.vsWater),
        (lightningAdv, sigil.vsLightning),
        (woodAdv, sigil.vsWood),
        (iceAdv, sigil.vsIce),
        (primalAdv, sigil.vsPrimal),
        (ghostAdv, sigil.vsGhost),
        (arcaneAdv, sigil.vsArcane)
    };

        foreach (var (image, finalValue) in entries)
        {
            image.sprite = modEmpty;
            image.preserveAspect = true;
            yield return delay;

            for (int i = 1; i <= finalValue; i++)
            {
                image.sprite = modSprites[i - 1];
                image.preserveAspect = true;

                if (modSounds != null && modSounds.Length >= i && modSounds[i - 1] != null && advantageAudioSource != null)
                {
                    advantageAudioSource.PlayOneShot(modSounds[i - 1]);
                }

                yield return delay;
            }

            yield return delay;
        }
    }
    private IEnumerator HideDuplicateWarningAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (duplicateWarningGroup != null)
        {
            float fadeDuration = 0.5f;
            float elapsed = 0f;
            float startAlpha = duplicateWarningGroup.alpha;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float newAlpha = Mathf.Lerp(startAlpha, 0f, elapsed / fadeDuration);
                duplicateWarningGroup.alpha = newAlpha;
                yield return null;
            }

            duplicateWarningGroup.alpha = 0f;
            duplicateWarningPanel.SetActive(false);
            duplicateWarningGroup.alpha = 1f;
        }
        else if (duplicateWarningPanel != null)
        {
            duplicateWarningPanel.SetActive(false);
        }
    }


    private IEnumerator DelayedAdvantageAnimation(Sigil sigil, float delay)
    {
        yield return new WaitForSeconds(delay);
        yield return AnimateAdvantagePanel(sigil);
    }

}
