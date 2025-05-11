using UnityEngine;

public class StageSelectAudio : MonoBehaviour
{
    public AudioSource sfxSource;
    public AudioSource musicSource;

    public AudioClip selectionClip;
    public AudioClip confirmClip;
    public AudioClip backgroundMusic;

    void Start()
    {
        if (musicSource != null && backgroundMusic != null)
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
}
