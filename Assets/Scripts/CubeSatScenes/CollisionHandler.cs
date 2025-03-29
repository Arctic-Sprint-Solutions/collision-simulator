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
    // private SatelliteExplosion explosionScript;
    // private FragmentMovement fragmentMovementScript;
    [SerializeField] private GameObject originalObject;
    [SerializeField] private GameObject fragmentObject;
    private bool collisionDetected = false;

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Finds the parent object and gets the SatelliteExplosion script.
    /// </summary>
    void Start()
    {
        // Find the parent object and get the SatelliteExplosion script
        // explosionScript = GetComponentInParent<SatelliteExplosion>();

        // Find the parent object and get the FragmentMovement script
        // fragmentMovementScript = GetComponentInParent<FragmentMovement>();

        if(fragmentObject != null)
        {
            Debug.Log("Fragment object found: " + fragmentObject.name);
            fragmentObject.SetActive(false);
        }

        if(originalObject != null)
        {
            Debug.Log("Original object found: " + originalObject.name);
        }
    }

    /// <summary>
    /// Called when the satellite collides with another object.
    /// If the object is tagged as "Debris", it triggers the detonation of the satellite's bomb.
    /// </summary>
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision detected with: " + collision.gameObject.name);
        if(!collisionDetected)
        {
            ActivateFragments();
        }
        // if(collision.gameObject.tag == "Debris" || collision.gameObject.tag == "Satellite")
        // {
        //     if(explosionScript != null)
        //     {
        //         // Trigger the detonation of the bomb
        //         explosionScript?.DetonateBomb(); 
        //     }

        //     if(fragmentMovementScript != null)
        //     {
        //         // Apply velocity to fragments
        //         fragmentMovementScript?.ApplyVelocityToFragments();
        //     }
        // }
    }

    public void ActivateFragments()
    {
        if (originalObject != null && fragmentObject != null)
        {
            collisionDetected = true;
            // Disable the original cube
            originalObject.SetActive(false);
            
            // Enable the fragmented version
            fragmentObject.SetActive(true);
            
            // If your Cube_root has a Rayfire Rigid component and you want to activate it
            RayFire.RayfireRigid rigidComponent = fragmentObject.GetComponent<RayFire.RayfireRigid>();
            if (rigidComponent != null)
            {
                rigidComponent.Activate();
            }
        }
    }

}
