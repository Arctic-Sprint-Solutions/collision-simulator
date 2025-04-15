using UnityEngine;

public class OrbitFromOrigin : MonoBehaviour
{
    public float OrbitRadius = 10f;
    public float OrbitSpeedDegrees = 30f;
    public float InclinationAngle = 0f;
    public float InitialAngle = 0f;
    public float rotationSmoothSpeed = 5f;

    public float GetCurrentAngle() => InitialAngle + OrbitSpeedDegrees * Time.time;

    private void Update()
    {
        float angle = GetCurrentAngle();
        Quaternion inclination = Quaternion.Euler(InclinationAngle, 0, 0);
        Vector3 offset = inclination * Quaternion.Euler(0, angle, 0) * Vector3.forward * OrbitRadius;

        transform.position = offset;


        Quaternion targetRotation = Quaternion.LookRotation(-offset.normalized, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothSpeed * Time.deltaTime);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        const int segments = 100;
        Vector3[] points = new Vector3[segments + 1];
        for (int i = 0; i <= segments; i++)
        {
            float angle = (360f / segments) * i;
            Quaternion inclination = Quaternion.Euler(InclinationAngle, 0, 0);
            Vector3 offset = inclination * Quaternion.Euler(0, angle, 0) * Vector3.forward * OrbitRadius;
            points[i] = offset;
        }

        for (int i = 0; i < segments; i++)
            Gizmos.DrawLine(points[i], points[i + 1]);
    }
#endif
}
