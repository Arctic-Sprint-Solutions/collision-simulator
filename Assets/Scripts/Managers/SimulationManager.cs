// Description: Manages the state of the simulation and handles scene transitions.

using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Enum representing the different states of the simulation
/// </summary>
public enum SimulationState
{
  MainMenu,
  SelectSatellite,
  SatelliteSelected
}

/// <summary>
/// Singleton class to manage the simulation state and handle scene transitions
/// </summary>
public class SimulationManager : MonoBehaviour
{
  public static SimulationManager Instance { get; private set; }
  private SimulationState currentState;
  public SimulationState CurrentState => currentState;

  // Reference to the selected satellite (from the satellites grid scene)
  private Satellite _selectedSatellite;
  public Satellite SelectedSatellite => _selectedSatellite;

  private string _previousScene;
  public string PreviousScene => _previousScene;

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

    // Handle scene-specific logic here
    switch (scene.name)
    {
      case "MainMenu":
        currentState = SimulationState.MainMenu;
        UIManager.Instance.HideNavBar();
        Time.timeScale = 1f;
        break;
      case "SettingsScene":
        UIManager.Instance.ShowNavBar();
        break;
      case "SpaceDebrisScene":
        UIManager.Instance.ShowNavBar();
        break;
        case "AboutScene":
        UIManager.Instance.ShowNavBar(backButtonKey: "Go Back");
        break;
        case "SatellitesGridScene":
        currentState = SimulationState.SelectSatellite;
        UIManager.Instance.ShowNavBar();
        Time.timeScale = 1f;
        break;
        case "SatellitePreviewScene":
        currentState = SimulationState.SatelliteSelected;
        UIManager.Instance.ShowNavBar(backButtonKey: "Go Back");
        Time.timeScale = 1f;

        break;
      case "CubeSatCollisionScene":
      case "Cubesat2RuScene":
      case "RosettaCollisionScene":
      case "AuraSatColllisionSceneParticle":
      case "RosettaDebrisCollisionScene":
        currentState = SimulationState.SatelliteSelected;
        UIManager.Instance.ShowNavBar();

        break;
      case "Init":

        break;
      default:
        Debug.LogWarning("Unknown scene loaded: " + scene.name);
        break;
    }
  }

  public void LoadScene(string sceneName)
  {
    Debug.Log($"Loading scene: {sceneName}");
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