using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;
using UnityEditor.Rendering;


// Description: Manager for handling camera selection
// Added Cinemachine for improved camera control and functionality
public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }
    [SerializeField] CameraController cameraController;


    // Cameras - Cinemachine
    //private List<CinemachineCamera> cameras;
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

    // private void Start()
    // {
    //     FindCamerasInScene();
    //     NotifyUI();
    //     SetActiveCamera(0);
    // }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Hide camera dropdown initially
        cameraController?.HideDropdown();
        
        FindCamerasInScene();
        if(cameras.Count > 1)
        {
            NotifyUI();
        } 
    }
    /// <summary>
    /// Finds all cameras in the scene dynamically
    /// </summary>


    /// <summary>
    /// Finds all Cinemachine cameras in the scene dynamically
    /// </summary>
    private void FindCamerasInScene()
    {
        cameras.Clear();
        cameras.AddRange(FindObjectsByType<CinemachineCamera>(FindObjectsInactive.Include, FindObjectsSortMode.None));

        // Sort cameras by priority from lowest to highest
        cameras = cameras.ToList();

        Debug.Log($"Found {cameras.Count} Second check afterr list - Cinemachine cameras in the scene.");
    }

    /// <summary>
    /// Notifies the dropdown UI with the list of available camera names
    /// </summary>
    private void NotifyUI()
    {
        if (OnCamerasUpdated != null)
        {
            List<string> cameraNames = cameras.Select(c => c.name).ToList();
            OnCamerasUpdated.Invoke(cameraNames);
        }
    }


    /// <summary>
    /// Sets the active camera based on setting priority higher than default
    /// </summary>

    public void SetActiveCamera(int index)
    {
        if (index < 0 || index >= cameras.Count) return;


        foreach (var cam in cameras)
        {
            cam.Priority = 10; // Default priority
        }

        // Set  selected camera's priority higher to make it active
        cameras[index].Priority = 20; 

        activeCameraIndex = index;
        Debug.Log($"Active Camera: {cameras[activeCameraIndex].name}");
    }


    /// <summary>
    /// Sorts cameras based on priority in descending order to help CameraController UI
    /// </summary>
    public List<CinemachineCamera> SortCamerasByPriority()
    {
        
        return cameras
            .OrderByDescending(cam => cam.Priority)
            .Select(cam => cam.GetComponent<CinemachineCamera>())
            .Where(cam => cam != null)
            .ToList();
    }

}

