using UnityEngine;

public class OrbitalMovement : MonoBehaviour
{
    [SerializeField] private Transform planet;
    [SerializeField] private float orbitRadius = 120f; // Default to 1.2 units from center (0.7 units above surface)
    [SerializeField] private float orbitSpeed = 2f;
    [SerializeField] private float inclination = 95f; // 0 = equatorial, 90 = polar
    
    [SerializeField] private float startAngle = 225f; // Default starting angle
    private float currentAngle;
    
    void Start()
    {
        currentAngle = startAngle;
        // Set initial position
        UpdatePosition();
    }
    
    void Update()
    {
        // Update orbit angle
        currentAngle += orbitSpeed * Time.deltaTime;
        if (currentAngle >= 360f) currentAngle -= 360f;
        
        // Update satellite position
        UpdatePosition();
    }
    
    void UpdatePosition()
    {
        // First, create a position as if in equatorial orbit
        float x = orbitRadius * Mathf.Cos(currentAngle * Mathf.Deg2Rad);
        float z = orbitRadius * Mathf.Sin(currentAngle * Mathf.Deg2Rad);
        Vector3 equatorialPosition = new Vector3(x, 0, z);
        
        // Now rotate this position based on the inclination
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

    
    public void SetStartAngle(float angle)
    {
        currentAngle = angle;
        UpdatePosition();
    }
    
    // Update the orbit visualization when parameters change
    
    // This ensures the orbit line updates when parameters are changed in the editor
    void OnValidate()
    {
        UpdatePosition();
    }
}