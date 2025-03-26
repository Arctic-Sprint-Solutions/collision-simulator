using UnityEngine;
using UnityEngine.UIElements; 
using System.Collections.Generic;
using UnityEditor.Rendering;

// Description: Camera manager for selecting camera angles using a drop-down menu
public class CameraManagerNew : MonoBehaviour
{

    public static CameraManagerNew Instance { get; private set; }

    // List of cameras in scene
    private List<Camera> cameras = new List<Camera>(); 

    // Current active camera
    private Camera currentCamera; 
    public GameObject satellite;
    [SerializeField] private UIDocument uiDocument;
    private DropdownField cameraDropdown;



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
        // Get the DropdownField from the UI Document
        var root = uiDocument.rootVisualElement;
        cameraDropdown = root.Q<DropdownField>("CameraDropdown");

        if (cameraDropdown != null)
        {
            // Check if there are more than one camera in the scene and add cameras to drop down
            if (cameras.Count > 1)
            {
                List<string> cameraNames = new List<string>();
                for (int i = 0; i < cameras.Count; i++)
                {
                    cameraNames.Add("Camera " + (i + 1));
                }

                cameraDropdown.choices = cameraNames;
                cameraDropdown.RemoveFromClassList("unity-base-field");


                cameraDropdown.value = cameraNames[0]; 
                cameraDropdown.label = "Select Camera";

                // Register callback for value changes
                cameraDropdown.RegisterValueChangedCallback(evt =>
                {
                    OnDropdownValueChanged(cameraNames.IndexOf(evt.newValue));
                });

                // Show dropdown if cameras are present
                cameraDropdown.style.display = DisplayStyle.Flex;  
            }
            else
            {
                // Hide dropdown if less than 2 cameras (no need for drop down)
                cameraDropdown.style.display = DisplayStyle.None;  
            }
        }

        // Set the first camera as the default active camera
        SetActiveCamera(0);
    }



    //Checks how many cameras are in the scene
    private int GetNumberOfCamerasInScene()
    {
        Camera[] camerasInScene = Object.FindObjectsByType<Camera>(FindObjectsSortMode.None);
        return camerasInScene.Length;
    }


    // Function for finding cameras in each scene
    private void FindCamerasInScene()
    {
        // Find all cameras in the scene using the updated method
        cameras.Clear(); // Clear any previous cameras in the list
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

    // UI dropdown menu for camera selection
    private void OnDropdownValueChanged(int index)
    {
        // Change the active camera based on selection in dropdown menu
        SetActiveCamera(index);
    }
}
