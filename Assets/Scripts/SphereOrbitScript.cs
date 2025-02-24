using UnityEngine;

public class SpaceDebrisManager : MonoBehaviour
{
    [System.Serializable]
    public class DebrisCategory
    {
        public string name;
        public int realCount;  // Actual debris count
        public int maxParticles;  // Max particles we want to render
        public float minAltitude;
        public float maxAltitude;
        public Color debrisColor;
        [HideInInspector] public ParticleSystem particleSystem;
    }

    public DebrisCategory[] debrisCategories;
    public Material debrisMaterial; // Assign a glowing material in the Inspector

    private void Start()
    {
        foreach (var category in debrisCategories)
        {
            CreateDebrisSystem(category);
        }
    }

    void CreateDebrisSystem(DebrisCategory category)
    {
        // Create GameObject for debris system
        GameObject debrisObject = new GameObject(category.name);
        debrisObject.transform.SetParent(transform);

        // Add Particle System
        ParticleSystem ps = debrisObject.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.maxParticles = category.maxParticles;
        main.startSpeed = 0;  // No movement from emission
        main.startSize = 5f; // Adjust for debris visibility
        main.startColor = category.debrisColor;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.loop = true;
        main.playOnAwake = true;

        // Enable Emission
        var emission = ps.emission;
        emission.rateOverTime = 0; // No continuous emission

        // Enable Shape
        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = category.maxAltitude; // Spherical distribution
        shape.radiusThickness = (category.maxAltitude - category.minAltitude) / category.maxAltitude;

        // Enable Renderer
        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.material = debrisMaterial;
        renderer.renderMode = ParticleSystemRenderMode.Billboard;

        // Assign system to category
        category.particleSystem = ps;

        // Generate Initial Particles
        GenerateDebris(ps, category);
    }

    void GenerateDebris(ParticleSystem ps, DebrisCategory category)
    {
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[category.maxParticles];

        for (int i = 0; i < category.maxParticles; i++)
        {
            float altitude = Random.Range(category.minAltitude, category.maxAltitude);
            Vector3 position = Random.onUnitSphere * altitude;
            particles[i].position = position;
            particles[i].startColor = category.debrisColor;
            particles[i].startSize = 0.2f;
            particles[i].remainingLifetime = Mathf.Infinity; // Stay forever
        }

        ps.SetParticles(particles, particles.Length);
    }

    private void Update()
    {
        // Rotate the entire debris system to simulate orbit
        transform.Rotate(Vector3.up, 5f * Time.deltaTime);
    }
}
