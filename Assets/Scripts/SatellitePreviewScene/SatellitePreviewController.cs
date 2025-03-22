using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Controller for the satellite preview scene. 
/// Handles loading the selected satellite and setting up the UI buttons for collision scenes.
/// </summary>
public class SatellitePreviewController : MonoBehaviour
{
    [SerializeField] private Transform satelliteContainer;
    [SerializeField] private UIDocument uiDocument;
    private GameObject satellitePrefab;
    private VisualElement _rootElement;
    private Satellite _selectedSatellite;

    /// <summary>
    /// Initializes the root visual element from the UIDocument.
    /// </summary>
    private void OnEnable()
    {
        if(uiDocument == null)
        {
            Debug.LogError("UIDocument is not assigned in the inspector.");
            return;
        }

        // Get the root visual element of the UI document
        _rootElement = uiDocument.rootVisualElement;
        if (_rootElement == null)
        {
            Debug.LogError("Root element is null. Ensure the UIDocument is properly set up.");
            return;
        }
    }

    /// <summary>
    /// Sets up the buttons in the UI based on the selected satellite's collision scenes.
    /// </summary>
    private void SetupButtons()
    {
        // Find the button contaier
        VisualElement buttonContainer = _rootElement.Q<VisualElement>("ButtonContainer");
        if(buttonContainer == null)
        {
            Debug.LogError("ButtonContainer not found in the UI.");
            return;
        }

        // Iterate through the collision scenes of the selected satellite        
        foreach (var scene in _selectedSatellite.collisionScenes)
        {
            // Find the button for the scene
            var button = buttonContainer.Q<Button>(scene.sceneType.ToString());
            if(button == null)
            {
                Debug.LogError($"Button for {scene.sceneType} not found in the UI.");
                continue;
            }
            button.RemoveFromClassList("unity-button");
            button.RemoveFromClassList("unity-text-element");
            button.RemoveFromClassList("d-none");

            // Add a click event listener to the button
            button.clicked += () => {
                Debug.Log($"Button for scene {scene.sceneAsset.name} clicked.");
                SimulationManager.Instance.LoadScene(scene.sceneAsset.name);
            };
        }
    }

    /// <summary>
    /// Loads the selected satellite and sets up the buttons in the UI if the satellite exists.
    /// </summary>
    private void Start()
    {
        LoadSelectedSatellite();   
        if(_selectedSatellite == null)
        {
            Debug.LogError("Selected satellite is not assigned.");
            return;
        }
        SetupButtons();
    }

    /// <summary>
    /// Loads the selected satellite from the SimulationManager and instantiates its prefab.
    /// </summary>
    private void LoadSelectedSatellite()
    {
        // Get the selected satellite from the SimulationManager
        _selectedSatellite = SimulationManager.Instance.SelectedSatellite;
        
        if (_selectedSatellite == null)
        {
            Debug.LogError("Selected satellite is null.");
            return;
        }

        if(_selectedSatellite.satellitePrefab == null)
        {
            Debug.LogError("Satellite prefab is null.");
            return;
        }

        // Instantiate the satellite prefab into the satellite container
        satellitePrefab = Instantiate(_selectedSatellite.satellitePrefab, satelliteContainer);
    }

    /// <summary>
    /// Cleans up the instantiated satellite prefab when the object is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        if (satellitePrefab != null)
        {
            Destroy(satellitePrefab);
        }
    }
}
