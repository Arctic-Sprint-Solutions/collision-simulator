// Description: Manager for handling camera selection
// Uses Cinemachine library for improved camera functionality and angles

using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

/// <summary>
/// Singleton class to find and manage all Cinemachine cameras in the scene.
/// <summary>
public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }
    /// <summary>
    /// Reference to the CameraController script for managing camera dropdown UI
    /// </summary>
    [SerializeField] CameraController cameraController;

    /// <summary>
    /// List of all Cinemachine cameras in the scene
    /// </summary>
    private List<CinemachineCamera> cameras = new List<CinemachineCamera>();
    private int activeCameraIndex = 0;

    #region Events
    public delegate void CamerasUpdated(List<string> cameraNames);
    public static event CamerasUpdated OnCamerasUpdated;
    #endregion

    /// <summary>
    /// Sets up the singleton instance and ensures that only one instance of CameraManager exists.
    /// </summary>
    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Subscribes to the scene loaded event to handle camera updates
    /// </summary>
    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    
    /// <summary>
    /// Unsubscribes from the scene loaded event to prevent memory leaks
    /// </summary>
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    /// <summary>
    /// Handles the scene loaded event to find cameras in the new scene
    /// and notify the UI with the updated camera list.
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Hide the camera dropdown UI when a new scene is loaded
        cameraController?.HideDropdown();

        FindCamerasInScene();

        // If there are more than one camera, notify the UI
        if(cameras.Count > 1)
        {
            NotifyUI();
        } 
    }


    /// <summary>
    /// Finds all Cinemachine cameras in the scene dynamically
    /// </summary>
    private void FindCamerasInScene()
    {
        // Clear the previous list of cameras
        cameras.Clear();
        // Fin all cinemachine cameras in the scene 
        cameras.AddRange(FindObjectsByType<CinemachineCamera>(FindObjectsInactive.Include, FindObjectsSortMode.None));

        // Sort cameras by priority from highest to lowest
        cameras = cameras.OrderByDescending(c => c.Priority.Value).ToList();
        
        Debug.Log($"Found {cameras.Count} Second check afterr list - Cinemachine cameras in the scene.");
    }

    /// <summary>
    /// Notifies the dropdown UI with the list of available camera names
    /// </summary>
    private void NotifyUI()
    {
        if (OnCamerasUpdated != null)
        {
            Debug.Log("Notifying UI with camera names.");
            List<string> cameraNames = cameras.Select(c => c.name).ToList();
            // Invoke the event to notify the UI with the camera names
            OnCamerasUpdated.Invoke(cameraNames);
        }
    }


    /// <summary>
    /// Sets the active camera based on index
    /// </summary>
    public void SetActiveCamera(int index)
    {
        if (index < 0 || index >= cameras.Count) return;

        for (int i = 0; i < cameras.Count; i++)
        {
            cameras[i].enabled = (i == index);
        }

        activeCameraIndex = index;
        Debug.Log($"Active Camera: {cameras[activeCameraIndex].name}");
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

