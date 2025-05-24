using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    [Header("Default Scene To Load (optional)")]
    public string sceneToLoad = "Level1";

    public bool callNewGameAfterLoad = false;
    public AudioClip clickSFX;
    public float fadeDuration = 1f;
    public Image fadeImage;

    private AudioSource audioSource;
    private string sceneNameOverride = null;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(FadeIn());
    }

    // ✅ New public method you can assign in Button > OnClick
    public void LoadSceneWithFade(string sceneName)
    {
        sceneNameOverride = sceneName;
        StartCoroutine(LoadSceneRoutine());
    }

    // ✅ Optional fallback if you just want to use the inspector's default
    public void LoadSceneWithFade()
    {
        sceneNameOverride = null;
        StartCoroutine(LoadSceneRoutine());
    }

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
            GameManager gm = FindObjectOfType<GameManager>();
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
