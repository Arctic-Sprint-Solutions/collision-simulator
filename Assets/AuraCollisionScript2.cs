using UnityEngine;

public class LaunchSphere : MonoBehaviour
{
    public Rigidbody targetRigidbody; 
    public float moveSpeed = 5f; 

    void FixedUpdate()
    {
        if (targetRigidbody != null)
        {

            targetRigidbody.MovePosition(targetRigidbody.position + Vector3.down * Time.fixedDeltaTime * moveSpeed);
        }
        else
        {
            Debug.LogWarning("Target Rigidbody not assigned!", this);
        }
    }
}
