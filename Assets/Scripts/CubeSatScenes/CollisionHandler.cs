// Description: Handles collision detection for the satellite.
using UnityEngine;

/// <summary>
/// Handles collision detection for the satellite.
/// When a collision with debris is detected, it activates the fragments.
/// </summary>
public class CollisionHandler : MonoBehaviour
{
    [SerializeField] private GameObject originalObject;
    [SerializeField] private GameObject fragmentObject;
    [SerializeField] private bool destroyRigidbody = false;
    private bool collisionDetected = false;

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Finds the parent object and gets the SatelliteExplosion script.
    /// </summary>
    void Start()
    {

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
    /// </summary>
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision detected with: " + collision.gameObject.name);
        if(!collisionDetected)
        {
            Debug.Log("Collision detected with: " + collision.gameObject.name);

            Rigidbody rb = GetComponent<Rigidbody>();
            if(rb != null && destroyRigidbody)
            {
                // Remove the rigidbody component
                Destroy(rb);
            }

            ActivateFragments();
        }
    }

    /// <summary>
    /// Activates the fragments and disables the original object.
    /// </summary>
    public void ActivateFragments()
    {
        if (originalObject != null && fragmentObject != null)
        {
            collisionDetected = true;
            // Disable the original cube
            originalObject.SetActive(false);
            
            // Enable the fragmented version
            fragmentObject.SetActive(true);
            
            // If the fragment object has a Rayfire Rigid component and you want to activate it
            RayFire.RayfireRigid rigidComponent = fragmentObject.GetComponent<RayFire.RayfireRigid>();
            if (rigidComponent != null)
            {
                rigidComponent.Activate();
            }
        }
    }

}
