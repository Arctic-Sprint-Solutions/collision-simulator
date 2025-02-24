using UnityEngine;

public class EarthRotation : MonoBehaviour
{
    public float rotationSpeed = 0.3f;

    void Update()
    {
        transform.Rotate(Vector3.up * -rotationSpeed * Time.deltaTime);
    }
}
