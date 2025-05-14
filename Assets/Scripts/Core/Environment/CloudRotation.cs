using UnityEngine;

public class CloudRotation : MonoBehaviour
{
    public float rotationSpeed = 0.1f; // Very slow for realistic movement

    void Update()
    {
        // Rotate the cloud layer around the Y-axis
        transform.Rotate(new Vector3(0.7f, -1f, 0) * rotationSpeed * Time.deltaTime);
    }
}
