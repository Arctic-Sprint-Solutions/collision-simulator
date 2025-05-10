using UnityEngine;
using System.Runtime.InteropServices;

/// <summary>
/// VideoManager is responsible for managing video recording functionality in a Unity WebGL application.
/// It provides methods to start, stop, and save video recordings using JavaScript interop.
/// This class is designed to work specifically with Unity's WebGL platform.
/// It uses a singleton pattern to ensure only one instance exists throughout the application.
/// </summary>
public class VideoManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of VideoManager
    /// </summary>
    public static VideoManager Instance { get; private set; }
    
    #region WebGL JavaScript Plugin Interop
    [DllImport("__Internal")]
    private static extern bool InitializeVideoRecorder();
    
    [DllImport("__Internal")]
    private static extern bool StartVideoRecording();
    
    [DllImport("__Internal")]
    private static extern bool StopVideoRecording();
    
    [DllImport("__Internal")]
    private static extern bool SaveVideoRecording();
    #endregion

    #region Events
    public event System.Action OnRecordingStarted;
    public event System.Action OnRecordingStopped;
    public event System.Action OnRecordingSaved;
    #endregion

    /// <summary>
    /// Indicates whether the video recording is currently active
    /// </summary>
    private bool _isRecording = false;

    /// <summary>
    /// Public property to check if the video is currently being recorded
    /// </summary>
    public bool IsRecording => _isRecording;

    /// <summary>
    /// Initializes the singleton instance and sets up the video recording functionality
    /// </summary>
    private void Awake()
    {
        // Ensure that there is only one instance of VideoManager
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    /// <summary>
    /// Called when the script instance is being loaded.
    /// Initializes the video recorder if running in WebGL.
    /// </summary>
    private void Start()
    {        
        #if !UNITY_EDITOR && UNITY_WEBGL
        InitializeVideoRecorder();
        #endif
    }
    
    /// <summary>
    /// Starts the video recording if running in WebGL. Uses the JavaScript plugin to handle the recording.
    /// </summary>
    private void StartRecording()
    {
        if (_isRecording)
        {
            return;
        }
            
        #if !UNITY_EDITOR && UNITY_WEBGL
        StartVideoRecording();
        #endif

        _isRecording = true;
        OnRecordingStarted?.Invoke();
    }
    
    /// <summary>
    /// Stops the video recording if it is currently active. Uses the JavaScript plugin to handle the stopping.
    /// </summary>
    private void StopRecording()
    {
        if (!_isRecording)
        {
            return;
        }
                    
        #if !UNITY_EDITOR && UNITY_WEBGL
        StopVideoRecording();
        #endif

        _isRecording = false;
        OnRecordingStopped?.Invoke();
    }

    /// <summary>
    /// Saves the recorded video. Uses the JavaScript plugin to handle the download.
    /// </summary>
    private void SaveRecording()
    {        
        #if !UNITY_EDITOR && UNITY_WEBGL
        SaveVideoRecording();
        #endif

        OnRecordingSaved?.Invoke();
    }

    /// <summary>
    /// Toggles the video recording state. If currently recording, it stops; otherwise, it starts recording.
    /// </summary>
    public void ToggleRecording()
    {
        if (_isRecording)
        {
            StopRecording();
        }
        else
        {
            StartRecording();
        }
    }
    
    /// <summary>
    /// Saves the current recording if not already recording.
    /// </summary>
    public void SaveCurrentRecording()
    {
        if (_isRecording)
        {
            Debug.LogWarning("Cannot save recording while still recording.");
            return;
        }
        
        SaveRecording();
    }
    
    /// <summary>
    /// Callback method to be called from JavaScript when the recording is finished.
    /// </summary>
    public void OnRecordingFinished(string videoUrl)
    {
        // Todo: Update UI once the recording is finished
        Debug.Log("Recording finished: " + videoUrl);
    }

    /// <summary>
    /// Cleans up the singleton instance when the object is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        // Clean up the singleton instance
        if (Instance == this)
        {
            Instance = null;
        }
    }
}