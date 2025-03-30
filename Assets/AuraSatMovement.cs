using UnityEngine;

// Script for movement of debris orbiting planet 

public class AuraSatMovement : MonoBehaviour
{
    // Reference to planet object
    [SerializeField] private Transform planet;

    // Distance from planet
    [SerializeField] private float orbitRadius = 100;

    [SerializeField] private float moveSpeed = 10f;

    [SerializeField] private float startAngle = 0f;

    // Angle trajectory
    [SerializeField] private float angleRange = 90f;

    private float currentAngle;

    private void Start()
    {

        currentAngle = startAngle;
    }

    private void Update()
    {
        // Increase the angle over time based on speed
        currentAngle += moveSpeed * Time.deltaTime;

        // Moving in the opposite direction of debris in scene

        float angleOffset = Mathf.PingPong(currentAngle - startAngle, angleRange);

        // Conversion to radians
        float angleRad = (startAngle - angleOffset) * Mathf.Deg2Rad;

        // Calculate new position
        float x = planet.position.x + Mathf.Cos(angleRad) * orbitRadius;
        float z = planet.position.z + Mathf.Sin(angleRad) * orbitRadius;

        transform.position = new Vector3(x, transform.position.y, z);
    }
}


