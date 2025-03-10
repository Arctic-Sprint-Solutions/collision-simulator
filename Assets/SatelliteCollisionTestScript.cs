using RayFire;
using UnityEngine;


public class RayfireGunImpact : MonoBehaviour
{
    public float speed = 10f;  // Movement speed
    public float impactRadius = 1.5f;  // Radius of shatter effect
    public float impactForce = 100f;   // Force applied to fragments

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * speed; // Moves the gun forward
    }

    void OnCollisionEnter(Collision collision)
    {
        RayfireRigid target = collision.gameObject.GetComponent<RayfireRigid>();

        if (target != null)
        {
            target.Demolish(); // Trigger shattering

            // Apply explosion force to fragments within the radius
            Collider[] fragments = Physics.OverlapSphere(collision.contacts[0].point, impactRadius);
            foreach (Collider col in fragments)
            {
                Rigidbody fragRb = col.GetComponent<Rigidbody>();
                if (fragRb != null)
                {
                    fragRb.AddExplosionForce(impactForce, collision.contacts[0].point, impactRadius);
                }
            }

            // Destroy the gun on impact (optional)
            Destroy(gameObject);
        }
    }
}
