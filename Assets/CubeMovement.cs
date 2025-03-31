using UnityEngine;

// Testing velocity in collision
public class CubeMovement : MonoBehaviour
{
    public Vector3 moveDirection = new Vector3(1, 0, 0); 
    public float speed = 5f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = moveDirection * speed;
    }
}
