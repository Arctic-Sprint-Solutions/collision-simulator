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
        if(!collisionDetected)
        {
            Debug.Log("Collision detected with: " + collision.gameObject.name);
            ActivateFragments();

            // If the fragment object has a MoveSatellite script, start moving it
            MoveSatellite moveScript = gameObject.GetComponent<MoveSatellite>();
            if (moveScript != null)
            {
                moveScript.StartMoving();
            }
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
            
            // If your Cube_root has a Rayfire Rigid component and you want to activate it
            RayFire.RayfireRigid rigidComponent = fragmentObject.GetComponent<RayFire.RayfireRigid>();
            if (rigidComponent != null)
            {
                rigidComponent.Activate();
            }
        }
    }

}
