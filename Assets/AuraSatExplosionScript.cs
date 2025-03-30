using RayFire;
using UnityEngine;


public class AuraSatExplosionScript : MonoBehaviour
{
    [SerializeField] private RayfireBomb bomb; 

    private void OnCollisionEnter(Collision collision)
    {
        // Check collision with Debris
        if (collision.gameObject.CompareTag("DebrisTestObject"))
        {
            Debug.Log("AuraSatBody has collided with Debris! ");

            // Detonate the bomb and explode immediately
            if (bomb != null)
            {
                bomb.Explode(0f); 
            }
            else
            {
                Debug.LogError("Rayfire Bomb not assigned in Inspector!");
            }

            // Destroy AuraSatBody
            Destroy(gameObject);
        }
    }
}
