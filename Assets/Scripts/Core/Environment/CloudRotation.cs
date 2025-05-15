using UnityEngine;

/// <summary>
/// Controls the rotation of the cloud layer in the environment.
/// </summary>
public class CloudRotation : MonoBehaviour
{
    /// <summary>
    /// The speed at which the cloud layer rotates.
    /// </summary>
    public float rotationSpeed = 0.1f; // Very slow for realistic movement

    /// <summary>
    /// Rotates the cloud layer around the Y-axis at a specified speed.
    /// </summary>
    void Update()
    {
        // Rotate the cloud layer around the Y-axis
        transform.Rotate(new Vector3(0.7f, -1f, 0) * rotationSpeed * Time.deltaTime);
    }
}
