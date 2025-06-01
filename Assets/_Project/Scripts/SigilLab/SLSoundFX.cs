using UnityEngine;

/// <summary>
/// Manages and plays sound effects used in the Sigil Lab and UI systems.
/// Uses the Singleton pattern to provide global access.
/// </summary>
public class SLSoundFX : MonoBehaviour
{
    /// <summary>
    /// Global instance of the SLSoundFX manager.
    /// </summary>
    public static SLSoundFX Instance;

    [Header("Shard Interactions")]
    [Tooltip("Sound played when a shard is picked up.")]
    public AudioClip shardPickUpSFX;

    [Tooltip("Sound played when a shard is dropped.")]
    public AudioClip shardDropSFX;

    [Header("Sigil Actions")]
    [Tooltip("Sound played when a sigil is revealed.")]
    public AudioClip sigilRevealSFX;

    [Tooltip("Sound played when sigils are combined.")]
    public AudioClip sigilCombine;

    [Tooltip("Sound played when a sigil is equipped.")]
    public AudioClip equipSigil;

    [Tooltip("Sound played when a sigil slot is selected.")]
    public AudioClip slotSelect;

    [Header("UI Interactions")]
    [Tooltip("Sound played when turning a UI page.")]
    public AudioClip pageTurnSFX;

    [Tooltip("Sound played when an error occurs.")]
    public AudioClip error;

    private AudioSource source;

    /// <summary>
    /// Sets up the Singleton instance and initializes the audio source.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            source = GetComponent<AudioSource>();

            if (source == null)
                Debug.LogError("❌ Missing AudioSource on SLSoundFX GameObject.");
        }
        else
        {
            Destroy(gameObject); // Prevent duplicate singletons
        }
    }

    /// <summary>
    /// Plays the specified sound effect through the internal audio source.
    /// </summary>
    /// <param name="clip">The AudioClip to play.</param>
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && source != null)
        {
            source.PlayOneShot(clip);
        }
        else if (clip == null)
        {
            Debug.LogWarning("⚠️ Tried to play a null AudioClip.");
        }
    }
}
