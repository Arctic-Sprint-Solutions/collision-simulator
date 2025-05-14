using UnityEngine;
using RayFire;

// <summary>
// This script is used to apply linear and angular velocity to fragments
// </summary>
public class FragmentMovement : MonoBehaviour
{
    [SerializeField] RayfireRigid rb;
    // Velocity to be applied to the fragments after explosion
    [SerializeField] Vector3 linearVelocity = new Vector3(15, 0, 0);
    [SerializeField] Vector3 angularVelocity = new Vector3(0, 15f, 0);

    // <summary>
    // Check if the rigidbody is assigned
    // </summary>
    void Start()
    {
        // Check if the rigidbody is assigned
        if (rb == null)
        {
            Debug.LogError("RayfireRigid component is not assigned!");
            return;
        }

    }

    // <summary>
    // Apply linear and angular velocity to the fragments
    // </summary>
    public void ApplyVelocityToFragments()
    {
        // Check if Rayfire Rigid is present
        if (rb != null && rb.fragments != null)
        {
            Debug.Log("Applying velocity to fragments of: " + rb.gameObject.name);
            // Loop through all fragments created after the explosion
            foreach (RayfireRigid fragment in rb.fragments)
            {
                if (fragment != null && fragment.gameObject != null)
                {
                    // Check if the fragment's rigidbody is not destroyed
                    if (fragment.physics != null && fragment.physics.rb != null)
                    {
                        // Apply the velocity to the fragment's rigidbody
                        fragment.physics.rb.linearVelocity = linearVelocity;
                        fragment.physics.rb.angularVelocity = angularVelocity;
                    }
                    else
                    {
                        Debug.LogWarning("Fragment Rigidbody is null or destroyed: " + fragment.gameObject.name);
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("No fragments found or Rayfire Rigid is null!");
        }
    }
}
