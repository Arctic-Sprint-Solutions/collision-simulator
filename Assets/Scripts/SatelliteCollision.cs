using UnityEngine;

public class SatelliteCollision : MonoBehaviour
{
    public GameObject fracturedVersion;
    public GameObject intactVersion;
    public float explosionForce = 500f;
    public float explosionRadius = 10f;

    void Start()
    {
        fracturedVersion.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Satellite"))
        {
            intactVersion.SetActive(false);

            fracturedVersion.SetActive(true);

            foreach (Transform piece in fracturedVersion.transform)
            {
                Rigidbody rb = piece.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = false;
                    rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
                }
            }
        }
    }
}

