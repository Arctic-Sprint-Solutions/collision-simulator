using UnityEngine;
using UnityEngine.UIElements;

public class DebrisController : MonoBehaviour
{
    [SerializeField] private PersistentParticles BigParticleSystem;
    [SerializeField] private PersistentParticles MediumParticleSystem;
    [SerializeField] private PersistentParticles SmallParticleSystem;

    private UIDocument uiDocument;
    private Toggle toggleA;
    private Toggle toggleB;
    private Toggle toggleC;

    private void OnEnable()
    { 
        // Get UI Document
        uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null)
        {
            Debug.LogError("UIDocument component not found.");
            return;
        }

        // Get UI Root
        var root = uiDocument.rootVisualElement;
        if (root == null)
        {
            Debug.LogError("Root VisualElement not found.");
            return;
        }

        toggleA = root.Q<Toggle>("ToggleDebrisA");
        toggleB = root.Q<Toggle>("ToggleDebrisB");
        toggleC = root.Q<Toggle>("ToggleDebrisC");

        if (toggleA != null) toggleA.RegisterValueChangedCallback(evt => BigParticleSystem.SetParticlesActive(evt.newValue));
        if (toggleB != null) toggleB.RegisterValueChangedCallback(evt => MediumParticleSystem.SetParticlesActive(evt.newValue));
        if (toggleC != null) toggleC.RegisterValueChangedCallback(evt => SmallParticleSystem.SetParticlesActive(evt.newValue));

        BigParticleSystem.SetParticlesActive(false);
        MediumParticleSystem.SetParticlesActive(false);
        SmallParticleSystem.SetParticlesActive(false);
    }

    private void ToggleDebris(PersistentParticles particleSystem, bool isOn)
    {
        if (particleSystem != null)
        {
            particleSystem.FadeParticles(isOn);
        }
    }
}
