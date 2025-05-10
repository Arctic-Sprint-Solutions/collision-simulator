using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Manages the debris particle systems and UI interactions for the Space Debris Scene.
/// This class handles the toggling of different debris particle systems (large, medium, small)
/// and manages the visibility of the information panel within the UI.
/// </summary>
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
    private Label showInfoLabel;
    private Label hideInfoLabel;
    private bool isInfoPanelVisible = false;

    /// <summary>
    /// Runs when the script instance is being loaded.
    /// Initializes the UI elements and registers callbacks for toggles and buttons
    /// </summary>
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

        if (toggleA != null) 
        {
            // Regiister value changed callback for toggleA (large debris)
            toggleA.RegisterValueChangedCallback(evt => BigParticleSystem.SetParticlesActive(evt.newValue));
        }

        if (toggleB != null) 
        {
            // Register value changed callback for toggleB (medium debris)
            toggleB.RegisterValueChangedCallback(evt => MediumParticleSystem.SetParticlesActive(evt.newValue));
        }

        if (toggleC != null) 
        {
            // Register value changed callback for toggleC (small debris)
            toggleC.RegisterValueChangedCallback(evt => SmallParticleSystem.SetParticlesActive(evt.newValue));
        }

        // Get Info elements
        showInfoButton = root.Q<Button>("ShowInfoButton");
        closeInfoButton = root.Q<Button>("CloseInfoButton");
        infoContainer = root.Q<VisualElement>("InfoContainer");
        showInfoLabel = root.Q<Label>("ShowInfoLabel");
        hideInfoLabel = root.Q<Label>("HideInfoLabel");

        // Initially hide the info panel
        infoContainer?.AddToClassList("d-none");
    
        if (showInfoButton != null)
        {
            showInfoButton.RemoveFromClassList("unity-button");
            showInfoButton.RemoveFromClassList("unity-text-element");
            // Add the click event to the show info button to toggle the info panel
            showInfoButton.clicked += () => ToggleInfoPanel(!isInfoPanelVisible);
        }

        if (closeInfoButton != null)
        {
            closeInfoButton.RemoveFromClassList("unity-button");
            closeInfoButton.RemoveFromClassList("unity-text-element");
            // Add the click event to the close button
            closeInfoButton.clicked += () => ToggleInfoPanel(false);
        }

        // Hide the info label by default
        hideInfoLabel?.AddToClassList("d-none");

        // Show the info label by default
        showInfoLabel?.RemoveFromClassList("d-none");

        // Control the initial state of the particle systems
        BigParticleSystem.SetParticlesActive(true);
        MediumParticleSystem.SetParticlesActive(false);
        SmallParticleSystem.SetParticlesActive(false);

        // Hide the info panel by default
        isInfoPanelVisible = false;
    }

    /// <summary>
    /// Toggles the visibility of the info panel and sets the isInfoPanelVisible flag
    /// </summary>
    /// <param name="show">True to show the info panel, false to hide it</param>
    private void ToggleInfoPanel(bool show)
    {
        isInfoPanelVisible = show;
        if (show)
        {
            ShowInfoPanel();
        }
        else
        {
            HideInfoPanel();
        }
    }


    /// <summary>
    /// Shows the info panel and updates the button labels accordingly
    /// </summary>
    private void ShowInfoPanel()
    {
        infoContainer?.RemoveFromClassList("d-none");
        hideInfoLabel?.RemoveFromClassList("d-none");
        showInfoLabel?.AddToClassList("d-none");
    }

    /// <summary>
    /// Hides the info panel and updates the button labels accordingly
    /// </summary>
    private void HideInfoPanel()
    {
        infoContainer?.AddToClassList("d-none");
        hideInfoLabel?.AddToClassList("d-none");
        showInfoLabel?.RemoveFromClassList("d-none");
    }
}
