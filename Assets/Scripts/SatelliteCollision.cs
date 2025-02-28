using UnityEngine;

public class SatelliteCollision : MonoBehaviour
{
    public GameObject intactModel;
    public GameObject fracturedModel;
    public float explosionForce = 200f;
    public float explosionRadius = 4f;
    public float velocityInheritanceFactor = 0.8f;
    public float torqueStrength = 10f;
    public float separationForce = 0.5f;

    private Vector3 originalVelocity;

    void Start()
    {
        fracturedModel.SetActive(false);

        Rigidbody rb = intactModel.GetComponent<Rigidbody>();
        if (rb != null)
        {
            originalVelocity = rb.linearVelocity;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Satellite"))
        {
            intactModel.SetActive(false);

            fracturedModel.SetActive(true);

            foreach (Rigidbody rb in fracturedModel.GetComponentsInChildren<Rigidbody>())
            {
                if (rb != null)
                {
                    rb.interpolation = RigidbodyInterpolation.Interpolate; 
                    rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

                    Vector3 breakDirection = (rb.transform.position - transform.position).normalized;

                    rb.linearVelocity = (originalVelocity * velocityInheritanceFactor) + (breakDirection * explosionForce * 0.1f);
                    rb.AddForce(breakDirection * separationForce, ForceMode.Impulse);

                    Vector3 randomTorque = new Vector3(
                        Random.Range(-1f, 1f), 
                        Random.Range(-1f, 1f), 
                        Random.Range(-1f, 1f)
                    ) * torqueStrength;
                    rb.AddTorque(randomTorque, ForceMode.Impulse);
                }
            }
        }
    }
}