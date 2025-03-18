using RayFire;
using UnityEngine;


public class ShootPartcleSpaceDebrisScript : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform spawnPoint;
    public float shootForce = 20f;
    public float shatterRadius = 1.5f; 

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            Shoot();
        }
    }

    void Shoot()
    {

        GameObject projectile = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.linearVelocity = spawnPoint.forward * shootForce;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ShatterTarget"))
        {

            RayfireRigid rayfireRigid = collision.gameObject.GetComponent<RayfireRigid>();
            if (rayfireRigid != null)
            {

                rayfireRigid.Demolish();
            }


            Destroy(gameObject);
        }
    }
}
