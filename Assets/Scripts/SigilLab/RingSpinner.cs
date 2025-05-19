using UnityEngine;

public class RingSpinner : MonoBehaviour
{
    // Speed of rotation in degrees per second
    public float rotationSpeed = 100f;

    void Update()
    {
        // Rotate around Z-axis (UI elements are 2D, so we rotate in Z)
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}
