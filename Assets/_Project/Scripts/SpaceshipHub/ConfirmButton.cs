using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ConfirmButton : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource engineIdleSource;
    public AudioSource engineAccelSource;
    public AudioSource engineHushSource;
    public AudioSource uiAudioSource;
    public AudioClip confirmSFX;

    [Header("UI & FX")]
    public RectTransform confirmButtonRect;
    public RectTransform roomLabelGroupRect;

    public CanvasGroup fadeOverlay; // Fullscreen fade canvas group

    [Header("Ship Launch")]
    public Transform shipTransform;
    public Vector3 launchOffset = new Vector3(1000f, 0f, 0f);
    public float launchDuration = 1.5f;

    [Header("Sigil Lab Zoom")]
    public Camera mainCamera;
    public Transform sigilLabTarget;
    public float zoomDuration = 1.5f;
    public float zoomAmount = 3f;

    [Header("Scene")]
    public float delayBeforeSceneLoad = 2f;
    [Header("Collections Room Zoom")]
    public Transform collectionsTarget;
    [Header("Additional Room Zooms")]
    public Transform configurationsTarget;
    public Transform recordsTarget;

    private string sceneToLoad;

    public void OnClickConfirm()
    {
        sceneToLoad = RoomSelector.GetSelectedScene();
        if (uiAudioSource != null && confirmSFX != null)
        {
            uiAudioSource.PlayOneShot(confirmSFX);
        }
        string selectedRoomName = RoomSelector.GetCurrentRoomName();

        Debug.Log("sceneToLoad = " + sceneToLoad);
        Debug.Log("Room Name = " + selectedRoomName);

        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogWarning("Scene to load is empty!");
            return;
        }

        // UI Exit
        LeanTween.moveY(confirmButtonRect, -800f, 0.5f).setEaseInQuad();
        LeanTween.moveY(roomLabelGroupRect, 3000f, 0.5f).setEaseInQuad();

        // Handle behavior by room
        if (sceneToLoad == "PlanetSelect" || selectedRoomName == "Cockpit")
        {
            HandleShipTakeoff();
        }
        else if (sceneToLoad == "Sigil Lab" || selectedRoomName == "Sigil Lab")
        {
            ZoomFadeAndLoad(sigilLabTarget, Color.white);
        }
        else if (sceneToLoad == "Collections" || selectedRoomName == "Collections")
        {
            ZoomFadeAndLoad(collectionsTarget, Color.white);
        }
        else if (sceneToLoad == "Configurations" || selectedRoomName == "Configurations")
        {
            ZoomFadeAndLoad(configurationsTarget, Color.white);
        }
        else if (sceneToLoad == "Records" || selectedRoomName == "Records")
        {
            ZoomFadeAndLoad(recordsTarget, Color.white);
        }

        else
        {
            Debug.LogWarning("No behavior defined for: " + selectedRoomName);
        }
    }


    private void HandleShipTakeoff()
    {
        if (engineIdleSource != null && engineIdleSource.isPlaying)
            engineIdleSource.Stop();

        Debug.Log("ShipTransform: " + shipTransform);

        engineHushSource?.Play();
        engineAccelSource?.Play();

        if (shipTransform != null)
        {
            Vector3 targetPos = shipTransform.localPosition + launchOffset;

            LeanTween.moveLocal(shipTransform.gameObject, targetPos, launchDuration)
                .setEaseInQuad()
                .setOnComplete(() =>
                {
                    // Lock the ship's final position
                    shipTransform.localPosition = targetPos;
                });

            // ⏱ Fade out slightly after launch starts, but *before* the snap back could be seen
            float fadeDelay = Mathf.Max(launchDuration - 0.5f, 0.1f);
            LeanTween.delayedCall(fadeDelay, () =>
            {
                FadeScreen(Color.black, 0.6f);
            });
        }

        // Final scene load after everything
        Invoke(nameof(LoadScene), delayBeforeSceneLoad);
    }



    private void HandleSigilLabZoom()
    {
        if (sigilLabTarget == null || mainCamera == null)
        {
            Debug.LogError("Sigil Lab Zoom Failed: Missing references!");
            return;
        }

        Debug.Log("Zooming into Sigil Lab...");

        // Zoom camera in
        LeanTween.value(gameObject, mainCamera.orthographicSize, zoomAmount, zoomDuration)
            .setOnUpdate((float val) => mainCamera.orthographicSize = val)
            .setEaseInOutSine();

        // Move camera to target position
        LeanTween.move(mainCamera.gameObject, sigilLabTarget.position + new Vector3(0, 0, -10f), zoomDuration)
            .setEaseInOutSine();

        // 🔄 Sync fade to match zoomDuration
        FadeScreen(Color.white, zoomDuration);

        // Load the scene after both have finished
        LeanTween.delayedCall(zoomDuration, () => LoadScene());
    }
    private void ZoomFadeAndLoad(Transform target, Color fadeColor)
    {
        if (target == null || mainCamera == null)
        {
            Debug.LogError("ZoomFadeAndLoad failed: Missing target or main camera!");
            return;
        }

        Debug.Log("Zooming into: " + target.name);

        // Zoom camera in
        LeanTween.value(gameObject, mainCamera.orthographicSize, zoomAmount, zoomDuration)
            .setOnUpdate((float val) => mainCamera.orthographicSize = val)
            .setEaseInOutSine();

        // Move camera to target
        LeanTween.move(mainCamera.gameObject, target.position + new Vector3(0, 0, -10f), zoomDuration)
            .setEaseInOutSine();

        // Fade to screen color over same duration
        FadeScreen(fadeColor, zoomDuration);

        // Load scene when fade is complete
        LeanTween.delayedCall(zoomDuration, () => LoadScene());
    }



    private void FadeScreen(Color fadeColor, float duration)
    {
        if (fadeOverlay == null) return;

        fadeOverlay.GetComponent<Image>().color = fadeColor;
        fadeOverlay.alpha = 0f; // Ensure starting from transparent
        LeanTween.alphaCanvas(fadeOverlay, 1f, duration).setEaseOutQuad();
    }


    private void LoadScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
