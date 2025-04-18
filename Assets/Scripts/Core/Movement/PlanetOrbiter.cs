// Description: This script controls the orbital movement of a satellite around a planet object.
using UnityEngine;

// <summary>
// This script controls the orbital movement of a satellite around a planet object.
// </summary>
public class PlanetOrbiter : MonoBehaviour
{
    // The planet object to orbit around
    [SerializeField] private Transform planet;
    // Radius of the orbit
    [SerializeField] private float orbitRadius = 120f; 
    // Speed of the orbit in degrees per second
    [SerializeField] private float orbitSpeed = 2f;
    // Inclination of the orbit in degrees (0 = equatorial, 90 = polar)
    [SerializeField] private float inclination = 95f;
    // Starting angle of the orbit in degrees
    [SerializeField] private float startAngle = 225f;
    private float currentAngle;
    
    /// <summary>
    /// Initializes the satellite's position and angle.
    /// </summary>
    void Start()
    {
        // Set the initial angle to the specified start angle
        currentAngle = startAngle;
        // Set initial position
        UpdatePosition();
    }
    
    /// <summary>
    /// Updates the satellite's position and rotation each frame.
    /// </summary>
    void Update()
    {
        // Update orbit angle
        currentAngle += orbitSpeed * Time.deltaTime;
        if (currentAngle >= 360f) currentAngle -= 360f;
        
        // Update satellite position
        UpdatePosition();
    }
    
    /// <summary>
    /// Calculates and sets the satellite's position based on the current angle and inclination.
    /// </summary>
    void UpdatePosition()
    {
        // Calculate the position in the equatorial plane
        float x = orbitRadius * Mathf.Cos(currentAngle * Mathf.Deg2Rad);
        float z = orbitRadius * Mathf.Sin(currentAngle * Mathf.Deg2Rad);
        Vector3 equatorialPosition = new Vector3(x, 0, z);
        
        // Rotate the position by the inclination angle
        Quaternion inclinationRotation = Quaternion.Euler(inclination, 0, 0);
        Vector3 finalPosition = inclinationRotation * equatorialPosition;
        
        // Set the satellite's position
        transform.position = planet.position + finalPosition;
        
        // Calculate next position for the satellite to look at
        float nextAngle = currentAngle + 1f;
        float nextX = orbitRadius * Mathf.Cos(nextAngle * Mathf.Deg2Rad);
        float nextZ = orbitRadius * Mathf.Sin(nextAngle * Mathf.Deg2Rad);
        Vector3 nextEquatorialPosition = new Vector3(nextX, 0, nextZ);
        Vector3 nextFinalPosition = inclinationRotation * nextEquatorialPosition;
        
        // Make satellite look in the direction of travel
        transform.LookAt(planet.position + nextFinalPosition);
    }
}