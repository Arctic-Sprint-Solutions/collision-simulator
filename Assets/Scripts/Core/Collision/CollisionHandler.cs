// Description: Handles collision detection for the satellite, supporting both standard collisions and trigger zones.
using UnityEngine;

/// <summary>
/// Handles collision detection for the satellite.
/// Supports both physical collisions and trigger detection depending on collider settings.
/// When a collision or trigger is detected, it activates the fragments and deactivates the original object.
/// </summary>
public class CollisionHandler : MonoBehaviour
{
    /// <summary>
    /// The original intact satellite GameObject.
    /// </summary>
    [SerializeField] private GameObject originalObject;

    /// <summary>
    /// The pre-fragmented version of the satellite to activate upon collision.
    /// </summary>
    [SerializeField] private GameObject fragmentObject;

    /// <summary>
    /// Determines whether to destroy the Rigidbody on this object after collision.
    /// </summary>
    [SerializeField] private bool destroyRigidbody = false;

    /// <summary>
    /// Keeps track of whether a collision has already been detected to prevent duplicate activations.
    /// </summary>
    private bool collisionDetected = false;

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Finds the fragment and original objects, and ensures the fragments are initially inactive.
    /// </summary>
    void Start()
    {
        if (fragmentObject != null)
        {
            Debug.Log("Fragment object found: " + fragmentObject.name);
            fragmentObject.SetActive(false);
        }

        if (originalObject != null)
        {
            Debug.Log("Original object found: " + originalObject.name);
        }
    }

    /// <summary>
    /// Called when the satellite collides with another object using physical collision.
    /// Only called if colliders are not marked as triggers.
    /// </summary>
    /// <param name="collision">The collision information.</param>
    void OnCollisionEnter(Collision collision)
    {
        HandleCollision("collision with", collision.gameObject);
    }

    /// <summary>
    /// Called when another collider enters this collider marked as a trigger.
    /// Only called if either this or the other collider has "Is Trigger" enabled.
    /// </summary>
    /// <param name="other">The collider that triggered this event.</param>
    void OnTriggerEnter(Collider other)
    {
        HandleCollision("trigger with", other.gameObject);
    }

    /// <summary>
    /// Handles the shared logic for both collision and trigger events.
    /// Activates fragments and disables the original satellite.
    /// </summary>
    /// <param name="reason">A string describing the type of event (collision or trigger).</param>
    /// <param name="other">The other GameObject involved in the interaction.</param>
    private void HandleCollision(string reason, GameObject other)
    {
        if (!collisionDetected)
        {
            Debug.Log($"Fragments activated due to {reason}: {other.name}");

            if (destroyRigidbody)
            {
                Rigidbody rb = GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // Remove the Rigidbody component
                    Destroy(rb);
                }
            }

            ActivateFragments();
            collisionDetected = true;
        }
    }

    /// <summary>
    /// Activates the fragments and disables the original object.
    /// Also disables the collider on the container to avoid interference with physics after fragmentation.
    /// </summary>
    public void ActivateFragments()
    {
        if (originalObject != null && fragmentObject != null)
        {
            // Disable the original satellite model
            originalObject.SetActive(false);

            // Enable the pre-fragmented version
            fragmentObject.SetActive(true);

            // If the fragment object has a Rayfire Rigid component and you want to activate it
            RayFire.RayfireRigid rigidComponent = fragmentObject.GetComponent<RayFire.RayfireRigid>();
            if (rigidComponent != null)
            {
                // Ensure Rayfire is initialized before activation
                if (!rigidComponent.initialized)
                    rigidComponent.Initialize();

                // Activate physics and fragmentation
                rigidComponent.Activate();
            }
        }
    }
}
