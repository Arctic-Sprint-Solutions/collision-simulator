using UnityEngine;

// Script for movement of debris orbiting planet 

public class SatelliteMovementAuraSatScene : MonoBehaviour
{
    // Reference to planet object
    [SerializeField] private Transform planet;

    // Distance from planet
    [SerializeField] private float orbitRadius = 100f; 

    [SerializeField] private float moveSpeed = 5f;   

    [SerializeField] private float startAngle = 20f;  

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

        // Movement between startAngle and startAngle + angleRange
        float angleOffset = Mathf.PingPong(currentAngle - startAngle, angleRange);

        // Conversion to radians
        float angleRad = (startAngle - angleOffset) * Mathf.Deg2Rad;

        // Calculate new position
        float x = planet.position.x + Mathf.Cos(angleRad) * orbitRadius;
        float z = planet.position.z + Mathf.Sin(angleRad) * orbitRadius;

        // Apply new position
        transform.position = new Vector3(x, transform.position.y, z);
    }
}
