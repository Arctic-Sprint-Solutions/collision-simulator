using UnityEngine;
using System.Collections;

public class PersistentParticles : MonoBehaviour
{
    private ParticleSystem ps;
    private ParticleSystem.Particle[] particles;

    public float sphereRadius = 5f;
    public float randomOffset = 0.5f;
    public float particleSize;
    
    private bool isActive = false;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    private void Start()
    {
        InitializeParticles();
        SetParticlesActive(false); 
    }
    
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

    private Vector3 RandomPointNearSphere(float radius, float offset)
    {
        Vector3 basePosition = Random.onUnitSphere * radius;
        Vector3 variation = Random.insideUnitSphere * offset;
        return basePosition + variation;
    }

    public void SetParticlesActive(bool active)
    {
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


    public void FadeParticles(bool activate)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeEmission(activate));
    }

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
