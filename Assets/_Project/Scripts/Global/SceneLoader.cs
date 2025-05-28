using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Handles scene transitions with optional fade animations and sound effects.
/// Can be triggered from UI buttons and optionally starts a new game after loading.
/// </summary>
public class SceneLoader : MonoBehaviour
{
    [Header("Default Scene Settings")]
    [Tooltip("Scene to load if no scene is passed into LoadSceneWithFade(string).")]
    public string sceneToLoad = "Level1";

    [Tooltip("If true, calls GameManager.NewGame() after the scene loads.")]
    public bool callNewGameAfterLoad = false;

    [Header("Audio & Visual")]
    [Tooltip("Sound effect played when the transition begins.")]
    public AudioClip clickSFX;

    [Tooltip("Duration of the fade effect in seconds.")]
    public float fadeDuration = 1f;

    [Tooltip("UI Image used to display the fade in/out overlay.")]
    public Image fadeImage;

    private AudioSource audioSource;
    private string sceneNameOverride = null;

    /// <summary>
    /// Initializes the audio source and begins a fade-in animation on scene load.
    /// </summary>
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(FadeIn());
    }

    /// <summary>
    /// Loads a scene by name with fade effect. Useful for UI buttons.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load.</param>
    public void LoadSceneWithFade(string sceneName)
    {
        sceneNameOverride = sceneName;
        StartCoroutine(LoadSceneRoutine());
    }

    /// <summary>
    /// Loads the default scene defined in the inspector with fade effect.
    /// </summary>
    public void LoadSceneWithFade()
    {
        sceneNameOverride = null;
        StartCoroutine(LoadSceneRoutine());
    }

    /// <summary>
    /// Handles fade-out, audio, scene loading, and optional new game setup.
    /// </summary>
    private IEnumerator LoadSceneRoutine()
    {
        if (clickSFX != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSFX);
        }

        yield return FadeOut();

        string finalSceneName = string.IsNullOrEmpty(sceneNameOverride) ? sceneToLoad : sceneNameOverride;
        SceneManager.LoadScene(finalSceneName);

        yield return null;

        if (callNewGameAfterLoad)
        {
            GameManager gm = Object.FindFirstObjectByType<GameManager>(); // ✅ Replaces deprecated FindObjectOfType
            if (gm != null)
            {
                gm.NewGame();
            }
            else
            {
                Debug.LogWarning("GameManager not found after scene load.");
            }
        }
    }

    /// <summary>
    /// Performs a fade-out effect by gradually increasing the alpha of the fadeImage.
    /// </summary>
    private IEnumerator FadeOut()
    {
        float t = 0f;
        Color color = fadeImage.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            color.a = Mathf.Clamp01(t / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }
    }

    /// <summary>
    /// Performs a fade-in effect by gradually decreasing the alpha of the fadeImage.
    /// </summary>
    private IEnumerator FadeIn()
    {
        float t = 0f;
        Color color = fadeImage.color;
        color.a = 1f;
        fadeImage.color = color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            color.a = 1f - Mathf.Clamp01(t / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }
    }
}
