using UnityEngine;
using System.Collections.Generic;

public class AuraOrbitScript : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform rotationCenter;  // Assign the Sphere in Inspector
    public int maxPoints = 100;  // Number of points to store in the orbit
    private Queue<Vector3> orbitPoints = new Queue<Vector3>();

    void Start()
    {
        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));  // Simple blue material
        lineRenderer.startColor = Color.blue;
        lineRenderer.endColor = Color.blue;
    }

    void Update()
    {
        // Add the Aura's current position to the orbit trail
        if (orbitPoints.Count >= maxPoints)
            orbitPoints.Dequeue();  // Remove the oldest point

        orbitPoints.Enqueue(transform.position);

        // Update LineRenderer with new orbit positions
        lineRenderer.positionCount = orbitPoints.Count;
        lineRenderer.SetPositions(orbitPoints.ToArray());
    }
}