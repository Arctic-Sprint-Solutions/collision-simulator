// Description: Manager for handling camera selection
// Uses Cinemachine library for improved camera functionality and angles

using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

/// <summary>
/// Singleton class to handle camera selections
/// <summary>
public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }
    [SerializeField] CameraController cameraController;

    // Cameras - Cinemachine
    private List<CinemachineCamera> cameras = new List<CinemachineCamera>();
    private int activeCameraIndex = 0;

    public delegate void CamerasUpdated(List<string> cameraNames);
    public static event CamerasUpdated OnCamerasUpdated;


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
    /// Enables Scenemanager to load scene
    /// </summary>
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <summary>
    /// Enables Scenemanager to offload scene
    /// </summary>
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// Loads scene and notifies CameraController UI of cameras
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        cameraController?.HideDropdown();

        FindCamerasInScene();
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
        cameras.Clear();
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

}

