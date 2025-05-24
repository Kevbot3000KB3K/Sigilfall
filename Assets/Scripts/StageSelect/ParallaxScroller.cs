using UnityEngine;

/// <summary>
/// Controls a horizontally scrolling parallax layer using two repeating sprite parts.
/// Automatically repositions off-screen parts to create a seamless looping background.
/// </summary>
public class ParallaxLayer : MonoBehaviour
{
    [Header("Scroll Settings")]
    [Tooltip("Speed at which this layer scrolls. Higher = faster movement.")]
    public float scrollSpeed = 1f;

    private Transform[] parts;     // Holds the two child sprite pieces
    private float spriteWidth;     // Width of one sprite used to calculate repositioning

    /// <summary>
    /// Initializes the sprite pieces and calculates their width.
    /// </summary>
    void Start()
    {
        parts = new Transform[2];
        parts[0] = transform.GetChild(0);
        parts[1] = transform.GetChild(1);

        SpriteRenderer sr = parts[0].GetComponent<SpriteRenderer>();
        spriteWidth = sr.bounds.size.x;
    }

    /// <summary>
    /// Scrolls the layer and loops the sprite parts when they go off-screen.
    /// </summary>
    void Update()
    {
        // Move the entire layer to the left
        transform.position += Vector3.left * scrollSpeed * Time.deltaTime;

        // Calculate the left edge of the camera view
        float camLeftEdge = Camera.main.transform.position.x - (Camera.main.orthographicSize * Camera.main.aspect);

        // Loop each sprite part if it moves off screen
        foreach (Transform part in parts)
        {
            float partRightEdge = part.position.x + (spriteWidth / 2f);

            if (partRightEdge < camLeftEdge)
            {
                // Get the other sprite
                Transform other = part == parts[0] ? parts[1] : parts[0];

                // Move the off-screen part to the right of the other
                part.position = new Vector3(other.position.x + spriteWidth, part.position.y, part.position.z);
            }
        }
    }
}
