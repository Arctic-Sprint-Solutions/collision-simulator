using UnityEngine;
using System.Runtime.InteropServices;

public class VideoManager : MonoBehaviour
{
    public static VideoManager Instance { get; private set; }
    [DllImport("__Internal")]
    private static extern bool InitializeVideoRecorder();
    
    [DllImport("__Internal")]
    private static extern bool StartVideoRecording();
    
    [DllImport("__Internal")]
    private static extern bool StopVideoRecording();
    
    [DllImport("__Internal")]
    private static extern bool SaveVideoRecording();
    
    private bool _isRecording = false;
    public bool IsRecording => _isRecording;

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
        Debug.Log("VideoRecorderManager initialized");
    }
    
    private void Start()
    {        
        Debug.Log("VideoRecorderManager started");

        #if !UNITY_EDITOR && UNITY_WEBGL
        InitializeVideoRecorder();
        #endif
    }
    
    private void StartRecording()
    {
        Debug.Log("Starting video recording");
        if (_isRecording)
        {
            return;
        }
            
        #if !UNITY_EDITOR && UNITY_WEBGL
        StartVideoRecording();
        #endif

        _isRecording = true;
        UIManager.Instance?.HideDownloadButton();
        UIManager.Instance?.UpdateRecordButtonText("Stop Recording");
    }
    
    private void StopRecording()
    {
        if (!_isRecording)
        {
            return;
        }
            
        Debug.Log("Stopping video recording");
        
        #if !UNITY_EDITOR && UNITY_WEBGL
        StopVideoRecording();
        #endif

        _isRecording = false;
        UIManager.Instance?.ShowDownloadButton();
        UIManager.Instance?.UpdateRecordButtonText("Start Recording");
    }

    private void SaveRecording()
    {
        Debug.Log("Saving video recording");
        
        #if !UNITY_EDITOR && UNITY_WEBGL
        SaveVideoRecording();
        #endif

        UIManager.Instance?.HideDownloadButton();
    }

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
    
    public void SaveCurrentRecording()
    {
        if (_isRecording)
        {
            Debug.LogWarning("Cannot save recording while still recording.");
            return;
        }
        
        SaveRecording();
    }
    
    // This will be called from JavaScript when recording is complete
    public void OnRecordingFinished(string videoUrl)
    {
        Debug.Log("Recording finished: " + videoUrl);
    }
}