using UnityEngine;
using System.Collections;

/// <summary>
/// Manages a persistent particle system that can be activated, deactivated, and faded.
/// Creates a cloud of particles around a sphere with customizable properties.
/// </summary>
public class PersistentParticles : MonoBehaviour
{
    /// <summary>
    /// Reference to the particle system component.
    /// </summary>
    private ParticleSystem ps;

    /// <summary>
    /// Array to store and manipulate individual particles.
    /// </summary>
    private ParticleSystem.Particle[] particles;

    /// <summary>
    /// The radius of the sphere around which particles are generated.
    /// </summary>
    public float sphereRadius = 5f;

    /// <summary>
    /// Random offset applied to each particle's position from the sphere surface.
    /// </summary>
    public float randomOffset = 0.5f;

    /// <summary>
    /// Size of each individual particle.
    /// </summary>
    public float particleSize;

    /// <summary>
    /// Tracks whether the particle system is currently active.
    /// </summary>
    private bool isActive = false;

    /// <summary>
    /// Reference to the current fade coroutine to allow cancellation.
    /// </summary>
    private Coroutine fadeCoroutine;

    /// <summary>
    /// Initializes the particle system reference.
    /// </summary>
    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    /// <summary>
    /// Sets up the particles and initially deactivates them.
    /// </summary>
    private void Start()
    {
        InitializeParticles();
        SetParticlesActive(false);
    }

    /// <summary>
    /// Creates and configures all particles in the system with specified properties.
    /// </summary>
    private void InitializeParticles()
    {
        int maxParticles = ps.main.maxParticles;
        particles = new ParticleSystem.Particle[maxParticles];

        for (int i = 0; i < maxParticles; i++)
        {
            particles[i].position = RandomPointNearSphere(sphereRadius, randomOffset);
            particles[i].startColor = Color.white;
            particles[i].startSize = particleSize;
            particles[i].startLifetime = Mathf.Infinity;
            particles[i].remainingLifetime = Mathf.Infinity;
        }

        ps.SetParticles(particles, particles.Length);
    }


    /// <summary>
    /// Generates a random point near the surface of a sphere.
    /// </summary>
    /// <param name="radius">Radius of the sphere</param>
    /// <param name="offset">Maximum random offset from the sphere surface</param>
    /// <returns>A random position vector near the sphere</returns>
    private Vector3 RandomPointNearSphere(float radius, float offset)
    {
        Vector3 basePosition = Random.onUnitSphere * radius;
        Vector3 variation = Random.insideUnitSphere * offset;
        return basePosition + variation;
    }


    /// <summary>
    /// Activates or deactivates the particle system.
    /// </summary>
    /// <param name="active">Whether the particles should be active</param>
    public void SetParticlesActive(bool active)
    {
        if (ps == null) return;
        isActive = active;
        var emission = ps.emission;
        emission.enabled = active;

        if (!active)
        {
            ps.Clear();
            ps.Stop();
        }
        else
        {
            InitializeParticles();
            ps.Play();
        }
    }


    /// <summary>
    /// Gradually fades the particle system in or out.
    /// </summary>
    /// <param name="activate">True to fade in, false to fade out</param>
    public void FadeParticles(bool activate)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeEmission(activate));
    }

    /// <summary>
    /// Coroutine that gradually changes the emission rate over time.
    /// </summary>
    /// <param name="activate">True to increase emission rate, false to decrease it</param>
    /// <returns>IEnumerator for the coroutine</returns>
    private IEnumerator FadeEmission(bool activate)
    {
        var emission = ps.emission;
        float startRate = emission.rateOverTime.constant;
        float targetRate = activate ? 50f : 0f; // Adjust 50f as needed

        float duration = 1.0f; // Fade duration
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            float lerpRate = Mathf.Lerp(startRate, targetRate, timeElapsed / duration);
            emission.rateOverTime = lerpRate;
            yield return null;
        }

        emission.rateOverTime = targetRate;
        isActive = activate;
    }
}
