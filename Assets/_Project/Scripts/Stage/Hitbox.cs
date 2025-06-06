using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public AudioClip strikeSound; // Assign this in Inspector
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ball"))
        {
            // Play strike sound effect
            if (strikeSound != null)
            {
                audioSource?.PlayOneShot(strikeSound);
            }
        }
    }
}
