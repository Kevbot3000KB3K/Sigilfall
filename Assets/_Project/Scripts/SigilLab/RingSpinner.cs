using UnityEngine;

/// <summary>
/// Continuously rotates the attached GameObject around the Z-axis.
/// Useful for spinning UI elements like rings or dials.
/// </summary>
public class RingSpinner : MonoBehaviour
{
    [Tooltip("Speed of rotation in degrees per second.")]
    public float rotationSpeed = 100f;

    /// <summary>
    /// Called every frame. Rotates the GameObject around the Z-axis.
    /// </summary>
    private void Update()
    {
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}
