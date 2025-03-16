using UnityEngine;

//Script to move box collider for solar panel to register objects hitting the solar panel
public class SolarPanelBoxColliderScript : MonoBehaviour
{
    void Start()
    {
        BoxCollider[] colliders = GetComponents<BoxCollider>();

        if (colliders.Length > 0) 
        {
            BoxCollider targetCollider = colliders[0]; 
            targetCollider.center = new Vector3(1.0f, 0.5f, 0.0f);
        }
        else
        {
            Debug.LogWarning("No BoxCollider found on " + gameObject.name);
        }
    }
}


