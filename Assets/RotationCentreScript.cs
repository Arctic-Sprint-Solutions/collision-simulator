using UnityEngine;

public class RotationCentreScript : MonoBehaviour
{
    public float rotationSpeed = 30f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

    }
}

