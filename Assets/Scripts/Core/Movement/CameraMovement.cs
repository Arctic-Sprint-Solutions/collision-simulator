using UnityEngine;

/// <summary>
/// CameraMovement is a script that controls the camera's movement for the About Scene.
/// It zooms out the camera to a specified distance from the target object.
/// </summary>
public class CameraMovement : MonoBehaviour
{
    /// <summary>
    /// Camera as the target for the script
    /// </summary>
    public Transform target;
    /// <summary>
    /// The target distance to zoom out to
    /// </summary>
    public float targetDistance = 50f;
    /// <summary>
    /// The speed at which the camera zooms out
    /// </summary> 
    public float zoomSpeed = 0.1f;
    /// <summary>
    /// Reference to the new position of the camera
    /// </summary>
    private Vector3 newPosition;

    /// <summary>
    /// Initializes the camera's position based on the target's position and the specified distance.
    /// </summary>
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

    /// <summary>
    /// Updates the camera's position to smoothly zoom out to the target distance.
    /// </summary>
    void Update()
    {
        // Moves the camera to new zoomed out position
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * zoomSpeed);
    }
}

