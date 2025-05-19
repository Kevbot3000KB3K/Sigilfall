using UnityEngine;

public class SLSoundFX : MonoBehaviour
{
    public static SLSoundFX Instance;

    public AudioClip shardPickUpSFX;
    public AudioClip shardDropSFX;
    public AudioClip sigilRevealSFX;
    public AudioClip pageTurnSFX;
    public AudioClip equipSigil;
    public AudioClip slotSelect;
    public AudioClip sigilReveal;
    public AudioClip sigilCombine;
    public AudioClip error;


    private AudioSource source;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            source = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            source.PlayOneShot(clip);
        }
    }
}
