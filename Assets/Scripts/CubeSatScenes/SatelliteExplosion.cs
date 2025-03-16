// Description: This script is used to detonate a RayfireBomb
using UnityEngine;
using RayFire;

// <summary>
// This script is used to detonate a RayfireBomb
// </summary>
public class SatelliteExplosion : MonoBehaviour
{
    // Reference to the RayfireBomb and RayfireRigid components
    [SerializeField] RayfireBomb bomb;

    private bool isDetonated = false;


    // <summary>
    // Check if the bomb is assigned
    // </summary>
    void Start()
    {
        // Check if the bomb is assigned
        if (bomb == null)
        {
            Debug.LogError("Bomb component is not assigned!");
            return;
        }
    }

    // <summary>
    // Detotonate te bomb
    // </summary>
    public void DetonateBomb()
    {
        if (bomb != null && !isDetonated)
        {
            isDetonated = true;
            
            // Detonate the bomb
            bomb.Explode(0f); // Provide a delay of 0 seconds

            Debug.Log("Bomb detonated");
        }
    }
}
