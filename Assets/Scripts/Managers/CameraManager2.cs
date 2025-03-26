using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class CameraManager : MonoBehaviour
{
    // Singleton instance to be able to have an instance of Camera Manager in every scene.
    public static CameraManager Instance { get; private set; }

    // Store cameras in a list for finding
    public List<Camera> camerasInScene = new List<Camera>();  

    private void Awake()
    {
        // Ensure only one instance of CameraManager per scene.
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  
        }
        else
        {
            Destroy(gameObject); 
            return;
        }
    }

    private void Start()
    {
        // Detect cameras in the scene
        DetectCameras(); 

        
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; 
    }


    // Refreshes list for every scene change
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        DetectCameras(); 
    }

    void DetectCameras()
    {
        // Clear the list 
        camerasInScene.Clear();

        // Find all cameras in the scene and store them
        Camera[] foundCameras = Object.FindObjectsByType<Camera>(FindObjectsSortMode.InstanceID);
        camerasInScene.AddRange(foundCameras);

        // Print camera names for easier debugging
        Debug.Log($"Cameras in scene '{SceneManager.GetActiveScene().name}':");
        foreach (Camera cam in camerasInScene)
        {
            Debug.Log(cam.name);
        }
    }
}
