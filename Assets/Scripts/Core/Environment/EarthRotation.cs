using UnityEngine;

/// <summary>
/// Controls the rotation of the Earth in the environment.
/// </summary>
public class EarthRotation : MonoBehaviour
{
    /// <summary>
    /// The speed at which the Earth rotates.
    /// </summary>
    public float rotationSpeed = 0.3f;

    /// <summary>
    /// Rotates the Earth around the Y-axis at a specified speed.
    /// </summary>
    void Update()
    {
        transform.Rotate(Vector3.up * -rotationSpeed * Time.deltaTime);
    }
}
