using UnityEngine;

/// <summary>
/// Handle movement of the satellite.
/// </summary>
public class MoveSatellite : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private Vector3 direction = Vector3.forward;
    private bool isMoving = false;

    void Update()
    {
        // Check if the satellite is moving
        if (isMoving)
        {
            Move();
        }
    }

    /// <summary>
    /// Move the satellite in the specified direction.
    /// </summary>
    private void Move()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    /// <summary>
    /// Start moving the satellite.
    /// </summary>
    public void StartMoving()
    {
        Debug.Log("Starting to move the satellite: " + gameObject.name);
        isMoving = true;
    }
}
