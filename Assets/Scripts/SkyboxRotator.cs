using UnityEngine;

/// <summary>
/// Rotates the skybox slightly over time to create a dynamic effect.
/// This script is attached to a GameObject in the scene and continuously updates the rotation of the skybox material.
/// </summary>
public class SkyboxRotator : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = .2f;

    private void Update()
    {
        // Rotate the skybox over time
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * rotationSpeed);
    }
}
