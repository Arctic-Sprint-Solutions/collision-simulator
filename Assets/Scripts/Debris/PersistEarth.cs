using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the persistence of the Earth object across specific scenes.
/// This singleton ensures the Earth is not duplicated when switching between allowed scenes.
/// Used between the Main Menu, Space Debris Scene, and Settings Scene.
/// </summary>
public class PersistEarth : MonoBehaviour
{
    /// <summary>
    /// Static reference to the singleton instance.
    /// </summary>
    private static PersistEarth instance;

    /// <summary>
    /// Initializes the singleton pattern for the Earth object.
    /// </summary>
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Handles scene loading events to determine if the Earth should persist.
    /// Destroys the Earth object when entering scenes where it shouldn't exist.
    /// </summary>
    /// <param name="scene">The scene that was loaded</param>
    /// <param name="mode">The scene loading mode</param>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "MainMenu" && scene.name != "SpaceDebrisScene" && scene.name != "SettingsScene")
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Cleans up event subscriptions when this object is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}