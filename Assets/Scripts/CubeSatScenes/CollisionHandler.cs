// Description: Handles collision detection for the satellite.
using UnityEngine;

/// <summary>
/// Handles collision detection for the satellite.
/// When a collision with debris is detected, it triggers the detonation of the satellite's bomb.
/// </summary>
public class CollisionHandler : MonoBehaviour
{
    /// <summary>
    /// Reference to the SatelliteExplosion script on the parent object.
    /// </summary>
    private SatelliteExplosion explosionScript;

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Finds the parent object and gets the SatelliteExplosion script.
    /// </summary>
    void Start()
    {
        // Find the parent object and get the SatelliteExplosion script
        explosionScript = GetComponentInParent<SatelliteExplosion>();

        if (explosionScript == null)
        {
            Debug.LogError("SatelliteExplosion script not found on parent object!");
        }
    }

    /// <summary>
    /// Called when the satellite collides with another object.
    /// If the object is tagged as "Debris", it triggers the detonation of the satellite's bomb.
    /// </summary>
    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Debris")
        {
            Debug.Log("Collision with satellite detected");
            // Trigger the detonation of the bomb
            explosionScript?.DetonateBomb(); 
        }
        
    
    }
}
