using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;


public class SigilLabManager : MonoBehaviour
{
    public static SigilLabManager Instance;

    private float alembicStartY;
    private float ringStartY;
    private float shardSlotsStartY;
    private bool isInformReveal = false;

    [Header("Library Icon Target")]
    public RectTransform libraryIconTarget; // 📌 Drag your Library UI button here


    [Header("Post-Reveal")]
    public Button confirmButton; // ← Assign in Inspector

    public AlterSlot[] alterSlots;
    public Button combineButton;
    public GameObject sigilDisplayArea;
    public Image sigilDisplayImage; // Assign in Inspector
    public Transform alembic;
    public Transform spinningRing;
    public Transform shardSlotGroup;
    public Animator shimmerAnim;
    public GameObject shimmerOverlay;
    private Coroutine shimmerLoopRoutine;
    public Image whiteFlashImage;  // Assign the WhiteFlash UI Image in the Inspector

    [Header("Sigil Info UI")]
    public TextMeshProUGUI sigilNameText;
    public Image familyIcon;
    public GameObject[] difficultyStars; // array of 5
    public Image advantageIcon;
    public Image specialEffectIcon;
    public TextMeshProUGUI idNumberText;
    public Image[] ingredientIcons; // size 3
    public TextMeshProUGUI descriptionText;
    public GameObject infoUIPanel;
    public Image newNote;

    [Header("Reveal Panel Elements")]
    public RectTransform namePanel;
    public RectTransform descriptionPanel;
    public RectTransform difficultyPanel;
    public RectTransform familyPanel;
    public RectTransform ingredientsPanel;
    public RectTransform idNumberPanel;
    public RectTransform specialPanel;
    public RectTransform advantagePanel;
    public RectTransform newNotePanel;

    [Header("Library UI")]
    public RectTransform libraryPanel; // ← assign the whole Library GameObject (with RectTransform)
    public float libraryTweenDuration = 0.8f;

    void Awake() => Instance = this;

    void Start()
    {
        alembicStartY = alembic.localPosition.y;
        ringStartY = spinningRing.localPosition.y;
        shardSlotsStartY = shardSlotGroup.localPosition.y;
        combineButton.gameObject.SetActive(false);
    }

    public void CheckForFullAlters()
    {
        bool allFilled = alterSlots.All(slot => slot.currentShardSO != null);

        combineButton.gameObject.SetActive(allFilled);
    }

    public void CombineShards()
    {
        SLSoundFX.Instance?.PlaySFX(SLSoundFX.Instance.sigilCombine);

        var selectedShards = alterSlots.Select(s => s.currentShardSO).ToArray();
        Sigil result = SigilDatabase.Instance.GetSigilFromShards(selectedShards);

        // Disable combine button
        combineButton.gameObject.SetActive(false);

        // Calculate target position
        float targetY = 425f;
        float duration = 1.2f;

        int finishedTweens = 0;

        System.Action onComplete = () =>
        {
            finishedTweens++;
            if (finishedTweens >= 3)
            {
                ShowSigil(result);
            }
        };

        // Move each UI element to absolute Y = 425
        LeanTween.moveLocalY(alembic.gameObject, targetY, duration).setEaseOutCubic().setOnComplete(onComplete);
        LeanTween.moveLocalY(spinningRing.gameObject, targetY, duration).setEaseOutCubic().setOnComplete(onComplete);
        LeanTween.moveLocalY(shardSlotGroup.gameObject, targetY, duration).setEaseOutCubic().setOnComplete(onComplete);

    }

    void AnimatePanelIn(RectTransform panel, Vector2 targetPos, float delay = 0f)
    {
        panel.gameObject.SetActive(true); // ensure it's visible

        Vector2 startPos = panel.anchoredPosition;

        LeanTween.value(panel.gameObject, startPos, targetPos, 0.8f)
            .setEase(LeanTweenType.easeOutCubic) // smoother than Expo, less jitter than Back
            .setDelay(delay)
            .setOnUpdate((Vector2 val) =>
            {
                panel.anchoredPosition = val;
            })
            .setOnComplete(() =>
            {
                panel.anchoredPosition = targetPos; // ensure it snaps perfectly at the end
            });
    }





    void ShowSigil(Sigil sigil)
    {
        if (sigil == null)
        {
            Debug.Log("❌ No sigil was created.");
            return;
        }

        if (!isInformReveal)
        {
            PlayerSigilCollection.Instance.Discover(sigil);

            var family = sigil.family;
            SigilSlotUI[] slots = LibraryTabController.Instance.GetSlotArrayForFamily(family);
            LibraryTabController.Instance.PopulateFamilySlots(family, slots);
        }
        else
        {
            newNotePanel.gameObject.SetActive(false); // ❌ Disable "New!" if this was Inform
        }



        ResetRevealPanelPositions(); 

        Debug.Log($"Created Sigil: {sigil.sigilName}");
        sigilDisplayArea.SetActive(true);
        infoUIPanel.SetActive(true);
        StartCoroutine(AnimateSigilReveal(sigil));


        // 🌀 Animate panels in from their freshly reset positions
        // 🌀 Animate panels to final positions
        AnimatePanelIn(namePanel, new Vector2(-3.85f, 4f), 0f);
        AnimatePanelIn(descriptionPanel, new Vector2(-5f, -3.3f), 0.1f);
        AnimatePanelIn(difficultyPanel, new Vector2(-4.5f, 2.8f), 0.2f);
        AnimatePanelIn(familyPanel, new Vector2(-7.5f, 2.5f), 0.25f);
        AnimatePanelIn(ingredientsPanel, new Vector2(7.8f, 1.2f), 0.3f);
        AnimatePanelIn(idNumberPanel, new Vector2(6.5f, 3.8f), 0.35f);
        AnimatePanelIn(specialPanel, new Vector2(-7.25f, 0.4f), 0.4f);
        AnimatePanelIn(advantagePanel, new Vector2(-5.3f, 1.3f), 0.45f);
        if (!isInformReveal)
            AnimatePanelIn(newNotePanel, new Vector2(3f, 4f), 0.5f);




        // ✨ Fill UI fields
        sigilNameText.text = $"{sigil.sigilName} Sigil";
        familyIcon.sprite = sigil.family.familyIcon;
        advantageIcon.sprite = sigil.advantage.familyIcon;
        specialEffectIcon.sprite = sigil.special.effectIcon; // assuming this exists
        idNumberText.text = $"{sigil.IDNumber:D3}";
        descriptionText.text = sigil.description;

        // Ingredients
        ingredientIcons[0].sprite = sigil.ingredient1.icon;
        ingredientIcons[1].sprite = sigil.ingredient2.icon;
        ingredientIcons[2].sprite = sigil.ingredient3.icon;

        for (int i = 0; i < ingredientIcons.Length; i++)
            ingredientIcons[i].preserveAspect = true;

        // Difficulty stars
        for (int i = 0; i < difficultyStars.Length; i++)
            difficultyStars[i].SetActive(i < sigil.difficulty);

        // Shimmer + Sound
        shimmerOverlay.SetActive(true);
        shimmerAnim?.Play("Shimmer");
        if (!isInformReveal)
            SLSoundFX.Instance?.PlaySFX(SLSoundFX.Instance.sigilRevealSFX);


        confirmButton.gameObject.SetActive(true);

        if (shimmerLoopRoutine != null)
            StopCoroutine(shimmerLoopRoutine); // prevent duplicates

        shimmerLoopRoutine = StartCoroutine(PlayShimmerLoop());
        isInformReveal = false;

    }

    public void ShowLibrary()
    {
        // Make sure the panel is active before tweening
        libraryPanel.gameObject.SetActive(true);

        // Start from offscreen left (if not already)
        libraryPanel.anchoredPosition = new Vector2(-1920f, 0f);

        // Tween into view (x = 0)
        LeanTween.moveLocalX(libraryPanel.gameObject, 0f, libraryTweenDuration)
                 .setEaseOutCubic();

        // Optional: Add SFX
        SLSoundFX.Instance?.PlaySFX(SLSoundFX.Instance.pageTurnSFX);
    }
    public void HideLibrary()
    {
        LeanTween.moveLocalX(libraryPanel.gameObject, -1920f, libraryTweenDuration)
                 .setEaseInCubic()
                 .setOnComplete(() =>
                 {
                     libraryPanel.gameObject.SetActive(false);
                 });

        // Optional: Sound effect
        SLSoundFX.Instance?.PlaySFX(SLSoundFX.Instance.pageTurnSFX);
    }
    public void ResetSigilLab()
    {
        float duration = 1.2f;

        // 1. Move UI objects back to starting Y positions
        LeanTween.moveLocalY(alembic.gameObject, alembicStartY, duration).setEaseInCubic();
        LeanTween.moveLocalY(spinningRing.gameObject, ringStartY, duration).setEaseInCubic();
        LeanTween.moveLocalY(shardSlotGroup.gameObject, shardSlotsStartY, duration).setEaseInCubic();


        // 2. Hide sigil display + info panels
        sigilDisplayArea.SetActive(false);
        infoUIPanel.SetActive(false);
        shimmerOverlay.SetActive(false);
        confirmButton.gameObject.SetActive(false);

        // 3. Clear all alter slots
        foreach (var slot in alterSlots)
        {
            slot.Clear();
        }

        // 4. Reset UI positions of info panels (so next time they animate in fresh)
        ResetRevealPanelPositions();

        // 5. Play optional reset SFX
        SLSoundFX.Instance?.PlaySFX(SLSoundFX.Instance.pageTurnSFX);
    }
    private void ResetRevealPanelPositions()
    {
        namePanel.anchoredPosition = new Vector2(-1300f, 4f);
        descriptionPanel.anchoredPosition = new Vector2(-1300f, -3.3f);
        difficultyPanel.anchoredPosition = new Vector2(-1300f, 2.8f);
        familyPanel.anchoredPosition = new Vector2(-1300f, 2.5f);
        ingredientsPanel.anchoredPosition = new Vector2(1300f, 1.2f);
        idNumberPanel.anchoredPosition = new Vector2(1300f, 3.8f);
        specialPanel.anchoredPosition = new Vector2(-1300f, 0.4f);
        advantagePanel.anchoredPosition = new Vector2(-1300f, 1.3f);
        newNotePanel.anchoredPosition = new Vector2(16f, 4f);

        if (shimmerLoopRoutine != null)
        {
            StopCoroutine(shimmerLoopRoutine);
            shimmerLoopRoutine = null;
        }
        shimmerOverlay.SetActive(false);

    }
    private IEnumerator AnimateSigilReveal(Sigil sigil)
    {
        // Set the sigil sprite and make it transparent
        sigilDisplayImage.sprite = sigil.sigilSprite;
        sigilDisplayImage.color = new Color(1, 1, 1, 0f); // Transparent
        sigilDisplayImage.gameObject.SetActive(true);

        // White flash fade in
        whiteFlashImage.gameObject.SetActive(true);
        whiteFlashImage.color = new Color(1, 1, 1, 0f);

        LeanTween.value(whiteFlashImage.gameObject, 0f, 1f, 0.3f).setOnUpdate((float val) => {
            whiteFlashImage.color = new Color(1, 1, 1, val);
        });

        yield return new WaitForSeconds(0.3f);

        // Pop in the sigil (behind flash)
        LeanTween.value(sigilDisplayImage.gameObject, 0f, 1f, 0.3f).setOnUpdate((float val) => {
            sigilDisplayImage.color = new Color(1, 1, 1, val);
        });

        yield return new WaitForSeconds(0.3f);

        // Fade out flash
        LeanTween.value(whiteFlashImage.gameObject, 1f, 0f, 0.4f).setOnUpdate((float val) => {
            whiteFlashImage.color = new Color(1, 1, 1, val);
        });

        yield return new WaitForSeconds(0.4f);

        whiteFlashImage.gameObject.SetActive(false);
    }

    IEnumerator PlayShimmerLoop()
    {
        while (true)
        {
            shimmerOverlay.SetActive(true);
            shimmerAnim.Play("Shimmer", -1, 0f); // restart from beginning

            // Wait until animation length
            float shimmerDuration = shimmerAnim.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(shimmerDuration);

            shimmerOverlay.SetActive(false); // optional fade
            yield return new WaitForSeconds(3f);
        }
    }
    public void RevealSigilFromLibrary()
    {
        var slot = LibraryTabController.Instance.currentSelectedSlot;

        if (slot == null)
        {
            Debug.LogWarning("⚠️ No sigil selected in the library.");
            return;
        }

        Sigil selected = slot.sigilData;

        if (selected == null)
        {
            Debug.LogWarning("⚠️ Selected slot has no sigil data.");
            return;
        }

        // ❌ If the sigil is undiscovered, don't allow inform behavior
        if (!PlayerSigilCollection.Instance.HasDiscovered(selected))
        {
            Debug.Log("⛔ Cannot view undiscovered sigil.");
            SLSoundFX.Instance?.PlaySFX(SLSoundFX.Instance.error); // <- Make sure you have an error sound assigned
            return;
        }

        // ✅ Proceed as normal for discovered sigils:
        HideLibrary();

        // Move lab elements into reveal position
        float targetY = 425f;
        float duration = 1.2f;

        int finishedTweens = 0;
        System.Action onComplete = () =>
        {
            finishedTweens++;
            if (finishedTweens >= 3)
            {
                isInformReveal = true;
                ShowSigil(selected);

            }
        };

        LeanTween.moveLocalY(alembic.gameObject, targetY, duration).setEaseOutCubic().setOnComplete(onComplete);
        LeanTween.moveLocalY(spinningRing.gameObject, targetY, duration).setEaseOutCubic().setOnComplete(onComplete);
        LeanTween.moveLocalY(shardSlotGroup.gameObject, targetY, duration).setEaseOutCubic().setOnComplete(onComplete);

        foreach (var slotUI in alterSlots)
            slotUI.Clear();

        combineButton.gameObject.SetActive(false);
    }

    private IEnumerator AnimateSigilToLibraryAndReset()
    {
        Vector3 startScale = sigilDisplayImage.transform.localScale;
        Vector3 endScale = Vector3.zero;

        Vector3 startPos = sigilDisplayImage.transform.position;
        Vector3 endPos = libraryIconTarget.position;

        float duration = 0.8f;

        // Optional white flash overlay
        Image overlay = Instantiate(whiteFlashImage, sigilDisplayImage.transform.parent);
        overlay.gameObject.SetActive(true);
        overlay.color = new Color(1, 1, 1, 0f);
        overlay.transform.position = sigilDisplayImage.transform.position;
        overlay.transform.SetAsLastSibling(); // ensure it draws on top

        // Move and scale sigil
        LeanTween.move(sigilDisplayImage.gameObject, endPos, duration).setEaseInCubic();
        LeanTween.scale(sigilDisplayImage.gameObject, endScale, duration).setEaseInCubic();

        // Fade white overlay in during tween
        LeanTween.value(overlay.gameObject, 0f, 1f, duration * 0.5f).setOnUpdate((float val) =>
        {
            overlay.color = new Color(1, 1, 1, val);
        });

        yield return new WaitForSeconds(duration);

        Destroy(overlay.gameObject); // Clean up the flash
        StartCoroutine(AnimateSigilToLibraryAndReset());

    }
    public void EquipSigil()
    {
        if (LibraryTabController.Instance.currentSelectedSlot != null && LibraryTabController.Instance.currentSelectedSlot.sigilData != null)

        {
            PlayerData.Instance.EquipSigil(LibraryTabController.Instance.currentSelectedSlot.sigilData);

            SLSoundFX.Instance?.PlaySFX(SLSoundFX.Instance.equipSigil);
        }
        else
        {
            Debug.LogWarning("⚠️ No sigil selected to equip.");
            SLSoundFX.Instance?.PlaySFX(SLSoundFX.Instance.error); // Optional error feedback
        }
    }


}
