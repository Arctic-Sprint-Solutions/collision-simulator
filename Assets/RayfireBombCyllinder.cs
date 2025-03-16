using RayFire;
using UnityEngine;

// Script for using Rayfire Bomb to create an explosion effect after a particle has hit the satellite and created a hole.
// The explosion comes after hitting the object, because this mimics the speed at which particles hit the solar panel.

using UnityEngine;
using RayFire;

public class ShatterCylinderOnImpact : MonoBehaviour
{
    public RayfireBomb bomb;  // Reference to the RayfireBomb component

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the object hitting the collider has the "Particle" tag
        if (collision.gameObject.CompareTag("Particle"))
        {
            // Check if the bomb reference is not set (null)
            if (bomb == null)
            {
                // Log an error message to the console
                Debug.LogError("Rayfire Bomb component is missing on " + gameObject.name);
            }
            else
            {
                // Log if the explosion happens successfully immediately upon play
                Debug.Log("Explosion triggered on " + gameObject.name);
                bomb.Explode(0f);  
            }
        }
    }
}
