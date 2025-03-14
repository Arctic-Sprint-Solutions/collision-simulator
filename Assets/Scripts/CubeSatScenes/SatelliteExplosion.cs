using UnityEngine;
using RayFire;

public class SatelliteExplosion : MonoBehaviour
{
    [SerializeField] RayfireBomb bomb;
    [SerializeField] RayfireRigid rb;

    [SerializeField] Vector3 linearVelocity = new Vector3(15, 0, 0);
    [SerializeField] Vector3 angularVelocity = new Vector3(0, 15f, 0);
    
    private bool isDetonated = false;

    void Start()
    {
        // Check if the bomb is assigned
        if (bomb == null)
        {
            Debug.LogError("Bomb component is not assigned!");
            return;
        }

        // Check if the rigidbody is assigned
        if (rb == null)
        {
            Debug.LogError("RayfireRigid component is not assigned!");
            return;
        }

    }

    void Update()
    {
        if (bomb == null || rb == null)
        {
            return;
        }
        // Check for space bar press to detonate
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DetonateBomb();
        }
    }

    public void DetonateBomb()
    {
        if (bomb != null && rb != null && !isDetonated)
        {
            isDetonated = true;
            
            // Detonate the bomb
            bomb.Explode(0f); // Provide a delay of 0 seconds
            ApplyVelocityToFragments();


            Debug.Log("Bomb detonated");
        }
    }

    // Function to apply velocity to the fragments after the explosion
    void ApplyVelocityToFragments()
    {
        // Check if Rayfire Rigid is present
        if (rb != null && rb.fragments != null)
        {
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
