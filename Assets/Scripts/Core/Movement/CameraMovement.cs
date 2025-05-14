using UnityEngine;

// Description: Camera script for About Scene to zoom out on start
public class CameraMovement : MonoBehaviour
{
    // Camera as the target for the script
    public Transform target;               
    public float targetDistance = 50f;     
    public float zoomSpeed = 0.1f;          
    private Vector3 newPosition;

    void Start()
    {
        if (target != null)
        {
            // New position set in comparison to the old camera position
            newPosition = target.position - transform.forward * targetDistance;
        }
        else
        {
            newPosition = transform.position - transform.forward * targetDistance;
        }
    }

    void Update()
    {
        // Moves the camera to new zoomed out position
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * zoomSpeed);
    }
}

