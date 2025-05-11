using UnityEngine;

/// <summary>
/// Rotates the GameObject along a specified axis at a configurable speed.
/// This script is intended for use with planets, clouds, satellites, debris, or any other rotating object.
/// </summary>
[DisallowMultipleComponent]
public class GlobalRotation : MonoBehaviour
{
    /// <summary>
    /// Rotation axis in local space. Example: (0, 1, 0) for Y-axis rotation.
    /// </summary>
    [Tooltip("Axis to rotate around (in local space). Example: (0,1,0) for Y-axis")]
    public Vector3 rotationAxis = Vector3.up;

    /// <summary>
    /// Rotation speed in degrees per second.
    /// </summary>
    [Tooltip("Rotation speed in degrees per second")]
    public float rotationSpeed = 1f;

    /// <summary>
    /// Rotates the GameObject around the specified axis every frame.
    /// </summary>
    void Update()
    {
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime, Space.Self);
    }
}
