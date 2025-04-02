using UnityEngine;
using System.Collections.Generic;
using System.Linq;


// Description: Manager for handling camera selection
public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    private List<Camera> cameras = new List<Camera>();
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

    private void Start()
    {
        FindCamerasInScene();
        NotifyUI();
        SetActiveCamera(0);
    }

    private void OnEnable()
    {
        CameraController.OnCameraSelected += SetActiveCamera;
    }

    private void OnDisable()
    {
        CameraController.OnCameraSelected -= SetActiveCamera;
    }

    /// <summary>
    /// Finds all cameras in the scene dynamically
    /// </summary>
    private void FindCamerasInScene()
    {
        cameras.Clear();
        cameras.AddRange(FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None));
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
