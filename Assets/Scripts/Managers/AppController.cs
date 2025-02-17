// Description: Initializes the application by creating persistent managers and loading the start scene

using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Class to initialize the game by creating persistent managers and loading the start scene
/// </summary>
public class AppController : MonoBehaviour
{
    /// <summary>
    /// Creates the persistent managers and loads the start scene
    /// </summary>
    private void Awake()
    {
        Debug.Log("AppController: Awake called");
        // Create the persistent managers object
        GameObject managers = new GameObject("Managers");

        // Add the SimulationManager component to this new object
        SimulationManager simulationManager = managers.AddComponent<SimulationManager>();

        // Make the managers object persistent
        DontDestroyOnLoad(managers);

        // Load the MainMenu scene
        SceneManager.LoadScene("MainMenu");

    }
}
