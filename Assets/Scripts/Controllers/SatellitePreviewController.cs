// Description: Controller for the satellite preview scene.
// Handles loading the selected satellite and setting up the UI buttons for collision scenes
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Enum representing different types of collision scenes.
/// </summary>
public enum SceneType
{
    SpaceDebrisCollision,
    SatelliteCollision
}

/// <summary>
/// Controller for the satellite preview scene. 
/// Handles loading the selected satellite and setting up the UI buttons for collision scenes.
/// </summary>
public class SatellitePreviewController : MonoBehaviour
{
    #region Properties
    /// <summary>
    /// Reference to the uiDocument component for accessing UI elements.
    /// </summary>
    [SerializeField] private UIDocument uiDocument;
    /// <summary>
    /// Reference to the list of satellite prefabs in the scene.
    /// </summary>
    private GameObject[] _satellitePrefabs;
    /// <summary>
    /// Reference to the root visual element of the UI document.
    /// </summary>
    private VisualElement _rootElement;
    /// <summary>
    /// Reference to the selected satellite.
    /// </summary>
    private Satellite _selectedSatellite;
    /// <summary>
    /// Reference to the load simulation button in the UI.
    /// </summary>
    private VisualElement _loadSimulationButton;
    /// <summary>
    /// Reference to the space debris button in the UI.
    /// </summary>
    private Button _spaceDebrisButton;
    /// <summary>
    /// Reference to the satellite collision button in the UI.
    /// </summary>
    private Button _satelliteCollisionButton;
    /// <summary>
    /// Reference to the selected scene name.
    /// </summary>
    private string _selectedScene;
    #endregion

    /// <summary>
    /// Initializes the visual elements and sets up the buttons when the script is enabled.
    /// </summary>
    private void OnEnable()
    {
        if (uiDocument == null)
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

        // Find the load simulation button in the UI
        _loadSimulationButton = _rootElement.Q<VisualElement>("LoadSimulationButton");
        if (_loadSimulationButton == null)
        {
            Debug.LogError("LoadSimulationButton not found in the UI.");
            return;
        }

        // Find the space debris button in the UI
        _spaceDebrisButton = _rootElement.Q<Button>("SpaceDebrisCollision");
        if (_spaceDebrisButton == null)
        {
            Debug.LogError("SpaceDebrisCollision button not found in the UI.");
            return;
        }

        // Find the satellite collision button in the UI
        _satelliteCollisionButton = _rootElement.Q<Button>("SatelliteCollision");
        if (_satelliteCollisionButton == null)
        {
            Debug.LogError("SatelliteCollision button not found in the UI.");
            return;
        }

        // Find all satellite prefabs (tag satellite)
        _satellitePrefabs = GameObject.FindGameObjectsWithTag("Satellite");
    }

    /// <summary>
    /// Deactivates all satellite prefabs and loads the selected satellite.
    /// Resets the buttons and sets up the info card and buttons in the UI.
    /// </summary>
    private void Start()
    {
        DeactivateAllPrefabs();
        LoadSelectedSatellite();
        if (_selectedSatellite == null)
        {
            Debug.LogError("Selected satellite is not assigned.");
            return;
        }
        SetupInfoCard();
        ResetButtons();
        SetupButtons();
    }

    /// <summary>
    /// Deactivates all satellite prefabs in the scene by setting their active state to false.
    /// Finds all game objects with the Satellite tag and deactivates them.
    /// </summary>
    private void DeactivateAllPrefabs()
    {
        // Look for all game objects with the Satellite tag
        if (_satellitePrefabs == null || _satellitePrefabs.Length == 0)
        {
            Debug.LogError("No satellite prefabs found.");
            return;
        }

        // Deactivate all satellite prefabs
        foreach (GameObject prefab in _satellitePrefabs)
        {
            prefab.SetActive(false);
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

        // Find the prefab for the selected satellite and activate it
        foreach (GameObject prefab in _satellitePrefabs)
        {
            if (prefab.name == _selectedSatellite.prefabName)
            {
                prefab.SetActive(true);
                break;
            }
        }
    }

    /// <summary>
    /// Sets up the info card in the UI with the selected satellite's information.
    /// </summary>
    private void SetupInfoCard()
    {
        // Find the info card in the UI
        VisualElement infoCard = _rootElement.Q<VisualElement>("SatelliteCard");
        if (infoCard == null)
        {
            Debug.LogError("Info card not found in the UI.");
            return;
        }

        // Set the title and subtitle of the info card
        SetLabel(infoCard, "Title", _selectedSatellite.satelliteName);
        SetLabel(infoCard, "Subtitle", _selectedSatellite.type);

        // Set the satellite infor text
        SetLabel(infoCard, "LEOText", _selectedSatellite.leoInfo + " km");
        SetLabel(infoCard, "WeightText", _selectedSatellite.weight.ToString() + " kg");
        SetLabel(infoCard, "YearText", _selectedSatellite.launchYear.ToString());
    }

    /// <summary>
    /// Resets the buttons by disabling them and hiding the load simulation button.
    /// </summary>
    private void ResetButtons()
    {
        // Remove unity classes
        _spaceDebrisButton.RemoveFromClassList("unity-button");
        _spaceDebrisButton.RemoveFromClassList("unity-text-element");
        _satelliteCollisionButton.RemoveFromClassList("unity-button");
        _satelliteCollisionButton.RemoveFromClassList("unity-text-element");

        // Disable all buttons in the button container
        _spaceDebrisButton.SetEnabled(false);
        _satelliteCollisionButton.SetEnabled(false);
        _loadSimulationButton.RemoveFromClassList("show-load-simulation");
    }

    /// <summary>
    /// Sets up the buttons in the UI based on the selected satellite's collision scenes.
    /// </summary>
    private void SetupButtons()
    {
        if (!string.IsNullOrEmpty(_selectedSatellite.debrisSceneName))
        {
            Debug.Log("Debris scene name: " + _selectedSatellite.debrisSceneName);
            _spaceDebrisButton.SetEnabled(true);
            _spaceDebrisButton.clicked += () => OnSelectCollisionClick(SceneType.SpaceDebrisCollision);
        }

        if (!string.IsNullOrEmpty(_selectedSatellite.satelliteSceneName))
        {
            _satelliteCollisionButton.SetEnabled(true);
            _satelliteCollisionButton.clicked += () => OnSelectCollisionClick(SceneType.SatelliteCollision);
        }

        // Hide the load simulation button by default
        _loadSimulationButton.RemoveFromClassList("show-load-simulation");
        // Register a callback for the load simulation button's click event
        _loadSimulationButton.RegisterCallback<ClickEvent>(e => OnLoadSimulationButtonClicked());
    }

    /// <summary>
    /// Handles the click event for the selected collision button.
    /// </summary>
    private void OnSelectCollisionClick(SceneType sceneType)
    {
        // Display the load simulation button
        _loadSimulationButton.AddToClassList("show-load-simulation");

        if (sceneType == SceneType.SpaceDebrisCollision)
        {
            _spaceDebrisButton.AddToClassList("selected");
            _satelliteCollisionButton.RemoveFromClassList("selected");
            _selectedScene = _selectedSatellite.debrisSceneName;
        }
        else
        {
            _satelliteCollisionButton.AddToClassList("selected");
            _spaceDebrisButton.RemoveFromClassList("selected");
            _selectedScene = _selectedSatellite.satelliteSceneName;
        }
    }

    /// <summary>
    /// Handles the click event for the load simulation button. Calls the SimulationManager to load the selected scene.
    /// </summary>
    private void OnLoadSimulationButtonClicked()
    {
        if (_selectedScene != null)
        {
            SimulationManager.Instance.LoadScene(_selectedScene);
        }
    }

    /// <summary>
    /// Sets the text of a label in a visual element.
    /// </summary>
    private void SetLabel(VisualElement parent, string labelName, string text)
    {
        var label = parent.Q<Label>(labelName);
        if (label != null)
        {
            label.text = text;
        }
    }

    /// <summary>
    /// Cleans up the instantiated satellite prefab when the object is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        // Unregister the click event for the load simulation button
        _loadSimulationButton.UnregisterCallback<ClickEvent>(e => OnLoadSimulationButtonClicked());
    }
}