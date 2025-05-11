using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeAndLoad : MonoBehaviour
{
    public AudioClip clickSound;
    public string sceneToLoad = "Global";
    public float fadeDuration = 1f;

    private AudioSource audioSource;
    private Image fadeImage;

    private void Start()
    {
        // Get the full screen black image
        fadeImage = GameObject.Find("FadeImage").GetComponent<Image>();

        // Optional: Fade in at the start
        StartCoroutine(FadeIn());

        // Setup AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void OnContinueButtonPressed()
    {
        // Start the fade and load coroutine
        StartCoroutine(FadeOutAndLoad());
    }

    private IEnumerator FadeOutAndLoad()
    {
        if (clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }

        // Fade to black
        float time = 0f;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Clamp01(time / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        // Wait briefly to let sound play fully
        yield return new WaitForSeconds(0.3f);

        // Load the scene
        SceneManager.LoadScene(sceneToLoad);
    }

    private IEnumerator FadeIn()
    {
        float time = 0f;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = 1 - Mathf.Clamp01(time / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
    }
}
