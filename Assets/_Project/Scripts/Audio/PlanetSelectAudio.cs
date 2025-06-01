using UnityEngine;
using System.Collections;

public class PlanetSelectAudio : MonoBehaviour
{
    public static PlanetSelectAudio Instance;

    public AudioSource sfxSource;
    public AudioSource musicSource;

    public AudioClip selectionClip;
    public AudioClip confirmClip;
    public AudioClip errorClip;
    public AudioClip backgroundMusic;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (musicSource != null && backgroundMusic != null && !musicSource.isPlaying)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void PlaySelectionSound()
    {
        if (sfxSource != null && selectionClip != null)
            sfxSource.PlayOneShot(selectionClip);
    }

    public void PlayConfirmSound()
    {
        if (sfxSource != null && confirmClip != null)
            sfxSource.PlayOneShot(confirmClip);
    }

    public void PlayErrorSound() 
    {
        if (sfxSource != null && errorClip != null)
            sfxSource.PlayOneShot(errorClip);
    }

    public void FadeOutMusic(float duration)
    {
        StartCoroutine(FadeOutCoroutine(duration));
    }

    private IEnumerator FadeOutCoroutine(float duration)
    {
        float startVolume = musicSource.volume;

        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            yield return null;
        }

        musicSource.Stop();
        musicSource.volume = startVolume; 
    }
}
