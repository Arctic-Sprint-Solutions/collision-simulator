using UnityEngine;
using System.Collections.Generic;


// Description: Camera manager for selecting cameras
public class CameraManagerNew : MonoBehaviour
{

    public static CameraManagerNew Instance { get; private set; }

    // List of cameras in scene
    private List<Camera> cameras = new List<Camera>(); 

    // Current active camera
    private Camera currentCamera; 
    public GameObject satellite;

    public delegate void CamerasUpdated(List<string> cameraNames);
    public static event CamerasUpdated OnCamerasUpdated;


    private void Awake()
    {
        if (Instance == null)
        {
            // Keeps the Camera Manager instance across all scenes
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Makes sure only one instance of the manager exists
            Destroy(gameObject);
            return;
        }

        // Finds cameras dynamically in the scene
        FindCamerasInScene();

    }

    private void Start()
    {
        FindCamerasInScene();
        NotifyUI();
        SetActiveCamera(0);
    }




    // Finds cameras dynamically in the scene
    private void FindCamerasInScene()
    {
        cameras.Clear(); 
        cameras.AddRange(Object.FindObjectsByType<Camera>(FindObjectsSortMode.None));
    }


    public void SetActiveCamera(int cameraIndex)
    {
        // If there are no cameras, return
        if (cameras.Count == 0) return;

        // Deactivate the current camera
        if (currentCamera != null)
        {
            currentCamera.gameObject.SetActive(false);
        }

        // Set the new camera as active
        currentCamera = cameras[cameraIndex];
        currentCamera.gameObject.SetActive(true);
    }



    // Function for notifying UI dropdown of list update
    private void NotifyUI()
    {
        if (cameras.Count > 0)
        {
            List<string> cameraNames = new List<string>();
            for (int i = 0; i < cameras.Count; i++)
            {
                cameraNames.Add("Camera " + (i + 1));
            }

            OnCamerasUpdated?.Invoke(cameraNames);
        }
    }
    private void OnEnable()
    {
        CameraDropdownUI.OnCameraSelected += SetActiveCamera;
    }

    private void OnDisable()
    {
        CameraDropdownUI.OnCameraSelected -= SetActiveCamera;
    }
}