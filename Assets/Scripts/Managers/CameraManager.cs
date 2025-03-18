
using UnityEngine;
using UnityEngine.UI;


// Script for creating a singleton for a PhotoManager

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    public Camera camera1;  // First preset camera
    public Camera camera2;  // Second preset camera

    public GameObject satellite;  // The satellite to focus on

    public Button button1;  // Button to activate Camera 1
    public Button button2;  // Button to activate Camera 2
    public Button button3;  // Future control option
    public Button button4;  // Future control option

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
        if (button1) button1.onClick.AddListener(() => SetActiveCamera(1));
        if (button2) button2.onClick.AddListener(() => SetActiveCamera(2));
        if (button3) button3.onClick.AddListener(Option3Function);
        if (button4) button4.onClick.AddListener(Option4Function);

        // Setting up Camera 1 as default camera
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
        camera1 = new GameObject("Camera1").AddComponent<Camera>();
        camera1.transform.position = satellite.transform.position + new Vector3(0, 10, -5);
        camera1.transform.LookAt(satellite.transform);

        // Default Position Camera 2
        camera2 = new GameObject("Camera2").AddComponent<Camera>();
        camera2.transform.position = satellite.transform.position + new Vector3(10, 0, -5);
        camera2.transform.LookAt(satellite.transform);
    }

    public void SetActiveCamera(int cameraNumber)
    {
        if (camera1 && camera2)
        {
            camera1.gameObject.SetActive(cameraNumber == 1);
            camera2.gameObject.SetActive(cameraNumber == 2);
        }
    }

    private void Option3Function()
    {
        Debug.Log("Option 3 not implemented yet.");
    }

    private void Option4Function()
    {
        Debug.Log("Option 4 not implemented yet.");
    }
}
