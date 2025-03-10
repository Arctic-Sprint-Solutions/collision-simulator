
using UnityEngine;
using RayFire;

public class LaunchDebris : MonoBehaviour
{
    public Rigidbody debrisRigidbody;
    public float launchForce = 10f;

    void Start()
    {
        // Apply forward force to the debris
        debrisRigidbody.linearVelocity = transform.forward * launchForce;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("AuraSat"))
        {
            // Get Rayfire Rigid component from the satellite
            RayfireRigid rayfireRigid = collision.gameObject.GetComponent<RayfireRigid>();
            if (rayfireRigid != null)
            {
                Debug.Log("Satellite hit! Demolishing..."); // Debug message to check collision

                // Apply shatter effect
                rayfireRigid.Demolish();
            }

            // Destroy debris after impact
            Destroy(gameObject);
        }
    }
}
