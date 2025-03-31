using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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
            return;
        }

        FindCamerasInScene();
    }

    private void Start()
    {
        FindCamerasInScene();
        NotifyUI();
        SetActiveCamera(0);
    }

    private void OnEnable()
    {
        UIManager.OnCameraSelected += SetActiveCamera;
    }

    private void OnDisable()
    {
        UIManager.OnCameraSelected -= SetActiveCamera;
    }

    // Finds all cameras in the scene dynamically
    private void FindCamerasInScene()
    {
        cameras.Clear();
        cameras.AddRange(FindObjectsOfType<Camera>());
    }

    // Notifies UIManager with the list of camera names
    private void NotifyUI()
    {
        if (OnCamerasUpdated != null)
        {
            List<string> cameraNames = cameras.Select(c => c.name).ToList();
            OnCamerasUpdated.Invoke(cameraNames);
        }
    }

    // Sets the active camera based on index
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
