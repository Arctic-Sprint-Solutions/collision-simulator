using UnityEngine;

public class CameraFollowSatellite : MonoBehaviour
{
    public Transform satellite;  // Assign LWORoot (Aura Satellite)
    public Vector3 closeOffset = new Vector3(0, 0.2f, -0.5f);  // Super close zoom
    public float followSpeed = 5f;  // Smooth follow speed

    void LateUpdate()
    {
        if (satellite == null) return;

        // Position the camera right behind the satellite
        Vector3 targetPosition = satellite.position + satellite.transform.TransformDirection(closeOffset);
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        // Make the camera look forward
        transform.LookAt(satellite.position + satellite.transform.forward * 2);
    }
}
