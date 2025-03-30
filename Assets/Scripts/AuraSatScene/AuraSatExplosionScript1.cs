using UnityEngine;
using RayFire;

public class AuraSatExplosionScript1 : MonoBehaviour
{
    [SerializeField] private RayfireBomb bomb;

    private bool isDetonated = false;

    private void Start()
    {
        if (bomb == null)
        {
            Debug.LogError("Bomb not assigned");
            return;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the colliding object has the tag "Debris"
        if (!isDetonated && collision.gameObject.CompareTag("DebrisTestObject"))
        {
            DetonateBomb();
        }
    }

    private void DetonateBomb()
    {
        if (bomb != null && !isDetonated)
        {
            isDetonated = true;
            bomb.Explode(0f); 
            Debug.Log("💥 Bomb detonated");
        }
    }
}
