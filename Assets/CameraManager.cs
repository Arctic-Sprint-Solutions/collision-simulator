using UnityEngine;
using TMPro;  // Built in tool to use drop down menu/ TMP_Dropdown


// Description: Camera manager for controlling camera angles using a drop-down menu
public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    public Camera camera1;  // First preset camera
    public Camera camera2;  // Second preset camera
    public Camera camera3;  // First preset camera - hover left?
    public Camera camera4;  // Second preset camera - hover right?
    public GameObject satellite;  // The satellite to focus on

    public TMP_Dropdown cameraDropdown;  // UI dropdown menu for camera selection

    private void Awake()
    {
        if (Instance == null)
        {
            // Keeps the object intact across all scenes
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Makes sure only one instance of the singleton exists in the simulation
            Destroy(gameObject);
            return;
        }

        CreateCameras();
    }

    private void Start()
    {
        if (cameraDropdown)
            cameraDropdown.onValueChanged.AddListener(OnDropdownValueChanged);

        // Setting up Camera 1 as the default camera
        SetActiveCamera(1);
    }

    private void CreateCameras()
    {
        if (!satellite)
        {
            Debug.LogError("Satellite not assigned in CameraManager.");
            return;
        }

        // Default Position Camera 1
        camera1 = new GameObject("Camera 1").AddComponent<Camera>();
        camera1.transform.position = satellite.transform.position + new Vector3(0, 10, -5);
        camera1.transform.LookAt(satellite.transform);

        // Default Position Camera 2
        camera2 = new GameObject("Camera 2").AddComponent<Camera>();
        camera2.transform.position = satellite.transform.position + new Vector3(10, 0, -5);
        camera2.transform.LookAt(satellite.transform);

        // Default Position Camera 3
        camera3 = new GameObject("Camera 3").AddComponent<Camera>();
        camera3.transform.position = satellite.transform.position + new Vector3(5, 10, -5);
        camera3.transform.LookAt(satellite.transform);

        // Default Position Camera 4
        camera4 = new GameObject("Camera 4").AddComponent<Camera>();
        camera4.transform.position = satellite.transform.position + new Vector3(-5, 0, -5);
        camera4.transform.LookAt(satellite.transform);
    }

    public void SetActiveCamera(int cameraNumber)
    {
        if (camera1 && camera2)
        {
            camera1.gameObject.SetActive(cameraNumber == 1);
            camera2.gameObject.SetActive(cameraNumber == 2);
            camera1.gameObject.SetActive(cameraNumber == 3);
            camera2.gameObject.SetActive(cameraNumber == 4);
        }
    }

    // UI dropdown menu for camera selection
    private void OnDropdownValueChanged(int index)
    {
        switch (index)
        {
            //Change cameras:
            case 0: SetActiveCamera(1); break; 
            case 1: SetActiveCamera(2); break; 
            case 2: SetActiveCamera(3); break;
            case 3: SetActiveCamera(4); break;
        }
    }
}
