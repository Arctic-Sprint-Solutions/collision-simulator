using UnityEngine;

//Script for creating a shatter effect on the Cyllinder debris when the particle hits the solar panel.

using UnityEngine;

public class ShatterOnCollision : MonoBehaviour
{
    public GameObject Cyllinder_root;  // Pre-shattered Cylinder_root
    private bool isShattered = false; 

    void Start()
    {
        // Ensure Cylinder_root is hidden initially
        if (Cyllinder_root != null)
        {
            Cyllinder_root.SetActive(false); 
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the colliding object is "Particle"
        if (collision.gameObject.CompareTag("Particle") && collision.gameObject.GetComponent<Rigidbody>())
        {

            if (!isShattered)
            {
                TriggerShattering();
                isShattered = true; 
            }
        }
    }

    void TriggerShattering()
    {
        // Activate pre-shattered cylinder upon collision
        if (Cyllinder_root != null)
        {
            Cyllinder_root.SetActive(true);  
        }

        // Optionally, you can apply forces or do more effects here.
        // For example, apply physics to the shattered parts, if required.

        // You can also trigger particle effects or sounds here.
    }
}
