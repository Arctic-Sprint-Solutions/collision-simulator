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
    private Button showInfoButton;
    private Button closeInfoButton;
    private VisualElement infoContainer;

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

        // Get toggles
        toggleA = root.Q<Toggle>("ToggleDebrisA");
        toggleB = root.Q<Toggle>("ToggleDebrisB");
        toggleC = root.Q<Toggle>("ToggleDebrisC");

        if (toggleA != null) toggleA.RegisterValueChangedCallback(evt => BigParticleSystem.SetParticlesActive(evt.newValue));
        if (toggleB != null) toggleB.RegisterValueChangedCallback(evt => MediumParticleSystem.SetParticlesActive(evt.newValue));
        if (toggleC != null) toggleC.RegisterValueChangedCallback(evt => SmallParticleSystem.SetParticlesActive(evt.newValue));

        // Get Info elements
        showInfoButton = root.Q<Button>("ShowInfoButton");
        closeInfoButton = root.Q<Button>("CloseInfoButton");
        infoContainer = root.Q<VisualElement>("InfoContainer");

        if (infoContainer != null)
        {
            infoContainer.style.display = DisplayStyle.None; // Initially hide the info panel
        }

        if (showInfoButton != null)
        {
            showInfoButton.clicked += () => ToggleInfoPanel(true);
        }

        if (closeInfoButton != null)
        {
            closeInfoButton.clicked += () => ToggleInfoPanel(false);
        }

        // Control the initial state of the particle systems
        BigParticleSystem.SetParticlesActive(true);
        MediumParticleSystem.SetParticlesActive(false);
        SmallParticleSystem.SetParticlesActive(false);
    }

    private void ToggleInfoPanel(bool show)
    {
        if (infoContainer != null)
        {
            infoContainer.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}
