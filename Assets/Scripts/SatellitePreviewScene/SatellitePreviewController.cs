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
    [SerializeField] private AppSettings appSettings;
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

        // Reset the buttons to hide them initially
        ResetButtons(buttonContainer);

        // Iterate through the collision scenes of the selected satellite        
        foreach (var scene in _selectedSatellite.collisionScenes)
        {
            Debug.Log($"Setting up button for scene: {scene.sceneType}");
            var collisionTitle = GetCollisionTitle(scene.sceneType);

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

            // Set the button text to the collision title if it exists
            if(collisionTitle != null)
            {
                Debug.Log($"Title for button: {collisionTitle}");
                button.text = collisionTitle;
            }

            // Add a click event listener to the button
            button.clicked += () => {
                Debug.Log($"Button for scene {scene.sceneAsset.name} clicked.");
                SimulationManager.Instance.LoadScene(scene.sceneAsset.name);
            };
        }
    }

    /// <summary>
    /// Resets the buttons in the button container by hiding them.
    /// <param name="buttonContainer">The container holding the buttons to reset.</param>
    /// </summary>
    private void ResetButtons(VisualElement buttonContainer)
    {
        // Iterate through all buttons in the button container
        foreach (var button in buttonContainer.Children())
        {
            if(button is Button btn)
            {
                // Add the "d-none" class to hide the button
                btn.AddToClassList("d-none");
            }
        }
    }

    /// <summary>
    /// Gets the collision title for a given scene type from the app settings.
    /// </summary>
    /// <param name="sceneType">The type of the collision scene.</param>
    /// <returns>The title for the collision scene, or null if not found.</returns>
    private string GetCollisionTitle(CollisionSceneType sceneType)
    {
        if(appSettings == null)
        {
            return null;
        }
        
        // Look for the scene type in the app settings
        var collisionTitle = appSettings.collisionTitles.Find(title => title.sceneType == sceneType);
        return collisionTitle?.collisionTitle ?? null;
            

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
        SetupInfoCard();
        SetupButtons();
    }

    /// <summary>
    /// Sets up the info card in the UI with the selected satellite's information.
    /// </summary>
    private void SetupInfoCard()
    {
        // Find the info card in the UI
        VisualElement infoCard = _rootElement.Q<VisualElement>("SatelliteCard");
        if(infoCard == null)
        {
            Debug.LogError("Info card not found in the UI.");
            return;
        }

        // Set the title and subtitle of the info card
        SetLabel(infoCard, "Title", _selectedSatellite.satelliteName);
        SetLabel(infoCard, "Subtitle", _selectedSatellite.type);

        // Set the satellite infor text
        SetLabel(infoCard, "LEOText", _selectedSatellite.leoInfo);
        SetLabel(infoCard, "WeightText", _selectedSatellite.weight.ToString());
        SetLabel(infoCard, "YearText", _selectedSatellite.launchYear.ToString());
    }

    /// <summary>
    /// Sets the text of a label in a visual element.
    /// </summary>
    private void SetLabel(VisualElement parent, string labelName, string text)
    {
        var label = parent.Q<Label>(labelName);
        if(label != null)
        {
            label.text = text;
        }
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
