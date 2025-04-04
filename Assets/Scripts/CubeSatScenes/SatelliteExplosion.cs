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
    [SerializeField] RayfireRigid rayfireRigid;

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

        // Check if the RayfireRigid component is assigned
        if (rayfireRigid == null)
        {
            Debug.LogError("RayfireRigid component is not assigned!");
            return;
        }
    }


    // <summary>
    // Listen for the spacebar key press to detonate the bomb
    // </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DetonateBomb();
        }
    }

    // <summary>
    // Detect collision and detonate the bomb
    // </summary>
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Debris" || collision.gameObject.tag == "Satellite")
        {
            Invoke(nameof(DetonateBomb), 0.2f); 
            Debug.Log("Collision detected with: " + collision.gameObject.name);
        }
    }

    // <summary>
    // Detotonate te bomb
    // </summary>
    public void DetonateBomb()
    {
        InitializeRayfireRigid();

        if (bomb != null && !isDetonated)
        {
            isDetonated = true;
            
            // Detonate the bomb
            bomb.Explode(0f); // Provide a delay of 0 seconds

            Debug.Log("Bomb detonated");
        }
    }

    // You can also create a custom method to trigger initialization
    private void InitializeRayfireRigid()
    {
        if (rayfireRigid != null)
        {
            Debug.Log("Activating RayfireRigid: " + rayfireRigid.name);
            rayfireRigid.Initialize();
        }
            
    }
}

