// Description: Manages the state of the simulation and handles scene transitions.

using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Enum representing the different states of the simulation
/// </summary>
public enum SimulationState
{
  MainMenu,
  SelectSatellite,
}

/// <summary>
/// Singleton class to manage the simulation state and handle scene transitions
/// </summary>
public class SimulationManager : MonoBehaviour
{
  public static SimulationManager Instance { get; private set; }
  private SimulationState currentState;
  public SimulationState CurrentState => currentState;

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
    // Handle scene-specific logic here
    switch (scene.name)
    {
      case "MainMenu":
        currentState = SimulationState.MainMenu;
        UIManager.Instance.HideNavBar();
        break;
      case "SatellitesGridScene":
        currentState = SimulationState.SelectSatellite;
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
    SceneManager.LoadScene(sceneName);
  }

  /// <summary>
  /// Unsubscribes from the scene loaded event when the object is destroyed
  /// </summary>
  private void OnDestroy()
  {
      SceneManager.sceneLoaded -= OnSceneLoaded;
  }
}