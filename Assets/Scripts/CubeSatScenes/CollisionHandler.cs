using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    private SatelliteExplosion explosionScript;

    void Start()
    {
        // Find the parent object and get the SatelliteExplosion script
        explosionScript = GetComponentInParent<SatelliteExplosion>();

        if (explosionScript == null)
        {
            Debug.LogError("SatelliteExplosion script not found on parent object!");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Debris")
        {
            Debug.Log("Collision with satellite detected");
            explosionScript?.DetonateBomb(); 
        }
        
    
    }
}
