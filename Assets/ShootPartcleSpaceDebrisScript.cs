using RayFire;
using UnityEngine;


public class ShootPartcleSpaceDebrisScript : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform spawnPoint;
    public float shootForce = 20f;
    public float shatterRadius = 1.5f; // Adjust for hole size

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left Click to shoot
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // Instantiate the sphere at the spawn point
        GameObject projectile = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);

        // Add Rigidbody force
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.linearVelocity = spawnPoint.forward * shootForce;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ShatterTarget"))
        {
            // Get the Rayfire Rigid component on the target
            RayfireRigid rayfireRigid = collision.gameObject.GetComponent<RayfireRigid>();
            if (rayfireRigid != null)
            {
                // Apply shatter effect at collision point
                rayfireRigid.Demolish();
            }

            // Destroy projectile after impact
            Destroy(gameObject);
        }
    }
}
