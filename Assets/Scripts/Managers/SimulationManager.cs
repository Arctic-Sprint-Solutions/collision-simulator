// Description: Manages the state of the simulation and handles scene transitions.

using UnityEngine;
using UnityEngine.SceneManagement;


#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Singleton class to manage the simulation state and handle scene transitions
/// </summary>
public class SimulationManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of SimulationManager
    /// </summary>
    public static SimulationManager Instance { get; private set; }

    #region  Private Properties
    /// <summary>
    /// Reference to the selected satellite
    /// </summary>
    private Satellite _selectedSatellite;
    /// <summary>
    /// Reference to the previous scene name
    /// </summary>
    private string _previousScene;
    /// <summary>
    /// Flag to indicate if the current scene is a collision scene.
    /// Default is false.
    /// </summary>
    private bool _isCollisionScene = false;
    #endregion

    #region  Public Properties
    /// <summary>
    /// Getter for the selected satellite
    /// </summary>
    public Satellite SelectedSatellite => _selectedSatellite;
    /// <summary>
    /// Getter for the previous scene name
    /// </summary>
    public string PreviousScene => _previousScene;
    /// <summary>
    /// Getter for the current scene name
    /// </summary>
    public bool IsCollisionScene => _isCollisionScene;
    #endregion

    #region  Events
    /// <summary>
    /// Event triggered when a collision scene is loaded
    /// </summary>
    public event System.Action OnCollisionSceneLoaded;
    /// <summary>
    /// Event triggered when a non-collision scene is loaded
    /// </summary>
    public event System.Action OnNonCollisionSceneLoaded;
    #endregion


    /// <summary>
    /// Initializes the singleton instance and subscribes to the scene loaded event
    /// </summary>
    private void Awake()
    {
        // Ensure that there is only one instance of SimulationManager
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Load the main menu scene
        SceneManager.LoadScene("MainMenu");
    }


    /// <summary>
    /// Handles scene-specific logic when a new scene is loaded
    /// <param name="scene">The loaded scene</param>
    /// <param name="mode">The mode in which the scene was loaded</param>
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene loaded: " + scene.name);

        // Hide the collision UI by default when a new scene is loaded
        UIManager.Instance.HideCollisionUI();
        _isCollisionScene = false;

        // Handle scene-specific logic
        switch (scene.name)
        {
            case "MainMenu":
                UIManager.Instance.HideNavBar();
                Time.timeScale = 1f;
                break;
            case "AboutScene":
            case "SatellitesGridScene":
            case "SettingsScene":
            case "SpaceDebrisScene":
                UIManager.Instance.ShowNavBar();
                Time.timeScale = 1f;
                break;
            case "SatellitePreviewScene":
                UIManager.Instance.ShowNavBar(true);
                Time.timeScale = 1f;
                break;
            case "AcrimSatScene_Debris":
            case "AcrimSatScene_Satellite":
            case "AuraSatScene_Debris":
            case "AuraSatScene_Satellite":
            case "CubesatScene_Debris":
            case "Cubesat2RuScene_Debris":
            case "Cubesat2RuScene_Satellite":
            case "IceSat2Scene_Debris":
            case "IceSat2Scene_Satellite":
            case "RosettaScene_Debris":
            case "RosettaScene_Satellite":
                UIManager.Instance.ShowNavBar();
                UIManager.Instance.ShowCollisionUI();
                _isCollisionScene = true;
                break;
            default:
                break;
        }

        // Notify listeners if the loaded scene is a collision scene or not
        if (_isCollisionScene)
        {
            OnCollisionSceneLoaded?.Invoke();
        }
        else
        {
            OnNonCollisionSceneLoaded?.Invoke();
        }
    }

    /// <summary>
    /// Loads a new scene and sets the previous scene
    /// </summary>
    public void LoadScene(string sceneName)
    {
        _previousScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Sets the selected satellite and loads the satellite preview scene
    /// </summary>
    public void SelectSatellite(Satellite satellite)
    {
        _selectedSatellite = satellite;
        // Debug.Log($"Selected satellite: {_selectedSatellite.collisionScenes[0].sceneAsset.name}");
        LoadScene("SatellitePreviewScene");
    }


    /// <summary>
    /// Quits the application or stops play mode in the Unity Editor
    /// </summary>
    public void QuitApplication()
    {
        Debug.Log("Quitting application...");

        #if UNITY_EDITOR
        // Stop play mode in the Unity Editor
        EditorApplication.isPlaying = false;
        #elif UNITY_WEBGL
        // Handle quit action for WebGL builds
        Debug.Log("Quit action triggered in WebGL build.");
        // Redirect to the start page
        Application.ExternalEval("window.location.href='index.html';");
        #else
        // Quit the application for standalone builds
        Application.Quit();
        #endif
    }

    /// <summary>
    /// Unsubscribes from the scene loaded event when the object is destroyed
    /// </summary>
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        // Clean up the singleton instance
        if (Instance == this)
        {
            Instance = null;
        }
    }
}