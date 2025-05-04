using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// VideoController is responsible for managing the video recording UI elements.
/// It handles the visibility of the record and download buttons, updates their states based on recording events,
/// and interacts with the VideoManager to start/stop recordings and download saved recordings.
/// </summary>
public class VideoController : MonoBehaviour
{
    private VisualElement _recordBtn;
    private VisualElement _downloadBtn;

    /// <summary>
    /// Initializes the VideoController and registers necessary events.
    /// </summary>
    private void Start()
    {
        if(UIManager.Instance != null) 
        {
            // Register UIManager events
            UIManager.Instance.OnCollisionSceneLoaded += InitializeCollisionScene;
            UIManager.Instance.OnNonCollisionSceneLoaded += InitializeNonCollisionScene;
            
            InitializeUI();
            Debug.Log("[VideoController] UI initialized successfully.");
        }

        // Register VideoManager events
        if(VideoManager.Instance != null)
        {
            VideoManager.Instance.OnRecordingStarted += RecordingStarted;
            VideoManager.Instance.OnRecordingStopped += RecordingStopped;
            VideoManager.Instance.OnRecordingSaved += RecordingSaved;

            Debug.Log("[VideoController] VideoManager events registered successfully.");
        }
    }

    /// <summary>
    /// Unregisters all events and cleans up resources when the object is disabled.
    /// </summary>
    private void OnDisable()
    {
        if(UIManager.Instance != null) 
        {
            // Unregister UIManager events
            UIManager.Instance.OnCollisionSceneLoaded -= ShowRecordButton;
            UIManager.Instance.OnNonCollisionSceneLoaded -= HideRecordButton;
        }

        // Unregister VideoManager events
        if(VideoManager.Instance != null)
        {
            VideoManager.Instance.OnRecordingStarted -= RecordingStarted;
            VideoManager.Instance.OnRecordingStopped -= RecordingStopped;
            VideoManager.Instance.OnRecordingSaved -= RecordingSaved;
        }
    }

    /// <summary>
    /// Initializes the UI elements for recording and downloading.
    /// </summary>
    private void InitializeUI()
    {
        // Get the record button from the UIManager
        _recordBtn = UIManager.Instance.GetElement<VisualElement>("RecordButton");
        if(_recordBtn != null)
        {
            var stopIcon = _recordBtn.Q<VisualElement>("StopIcon");
            var cameraIcon = _recordBtn.Q<VisualElement>("CameraIcon");
            stopIcon?.AddToClassList("d-none"); // Hide stop icon initially
            cameraIcon?.RemoveFromClassList("d-none"); // Show camera icon initially

            // Register the click event for the record button
            _recordBtn.RegisterCallback<ClickEvent>(evt => ToggleRecording());
        }

        // Get the download button from the UIManager
        _downloadBtn = UIManager.Instance.GetElement<VisualElement>("DownloadButton");
        if(_downloadBtn != null) 
        {   
            Debug.Log("Download button found in NavBar");
            _downloadBtn.RegisterCallback<ClickEvent>(evt => DownloadRecording());
            // Hide the download button initially
            _downloadBtn.AddToClassList("d-none");
        }
    }

    /// <summary>
    /// Initializes the UI for collision scenes.
    /// </summary>
    private void InitializeCollisionScene()
    {
        UpdateRecordButton(isRecording: false);
        ShowRecordButton();
    }

    /// <summary>
    /// Initializes the UI for non-collision scenes.
    /// </summary>
    private void InitializeNonCollisionScene()
    {
        UpdateRecordButton(isRecording: false);
        HideRecordButton();
    }

    /// <summary>
    /// Called when a recording is started.
    ///  Hides the download button and updates the record button to show the stop icon.
    /// </summary>
    private void RecordingStarted()
    {
        HideDownloadButton();
        UpdateRecordButton(isRecording: true);
    }

    /// <summary>
    /// Called when a recording is stopped.
    /// Shows the download button and updates the record button to show the camera icon.
    /// </summary>
    private void RecordingStopped()
    {
        ShowDownloadButton();
        UpdateRecordButton(isRecording: false);
    }

    /// <summary>
    /// Called when a recording is saved.
    /// Hides the download button to indicate that the recording has been saved.
    /// </summary>
    private void RecordingSaved()
    {
        HideDownloadButton();
    }

    /// <summary>
    /// Show the record button in the UI
    /// </summary>
    public void ShowRecordButton()
    {
        if(_recordBtn != null)
        {
            _recordBtn.RemoveFromClassList("d-none");
        }

        if (_downloadBtn != null)
        {
            _downloadBtn.AddToClassList("d-none");
        }
    }

    /// <summary>
    /// Hides the record button in the UI
    /// </summary>
    public void HideRecordButton()
    {
        if(_recordBtn != null)
        {
            _recordBtn.AddToClassList("d-none");
        }

        if (_downloadBtn != null)
        {
            _downloadBtn.AddToClassList("d-none");
        }
    }

    /// <summary>
    /// Shows the download button in the UI
    /// </summary>
    public void ShowDownloadButton()
    {
        if (_downloadBtn != null)
        {
            _downloadBtn.RemoveFromClassList("d-none");
            var label = _downloadBtn.Q<Label>("DownloadLabel");
            if (label != null)
            {
                label.text = LocalizedUIHelper.Get("DownloadRecording");
            }
        }
    }

    /// <summary>
    /// Hides the download button in the UI
    /// </summary>
    public void HideDownloadButton()
    {
        if (_downloadBtn != null)
        {
            _downloadBtn.AddToClassList("d-none");
        }
    }

    /// <summary>
    /// Updates the icon and the tect of the record button based on the recording state
    /// </summary>
    public void UpdateRecordButton(bool isRecording)
    {
        if (_recordBtn != null)
        {
            var stopIcon = _recordBtn.Q<VisualElement>("StopIcon");
            var cameraIcon = _recordBtn.Q<VisualElement>("CameraIcon");
            var label = _recordBtn.Q<Label>("RecordLabel");

            if (stopIcon == null || cameraIcon == null)
            {
                Debug.LogWarning("[UIManager] One or more recording UI elements are missing.");
                return;
            }
            if (isRecording)
            {
                stopIcon.RemoveFromClassList("d-none");
                cameraIcon.AddToClassList("d-none");
                label.text = LocalizedUIHelper.Get("StopRecording");
            }
            else
            {
                stopIcon.AddToClassList("d-none");
                cameraIcon.RemoveFromClassList("d-none");
                label.text = LocalizedUIHelper.Get("StartRecording");
            }
        }
    }

    /// <summary>
    /// Toggles the recording state using the VideoManager instance.
    /// </summary>
    private void ToggleRecording()
    {
        VideoManager.Instance?.ToggleRecording();
    }

     /// <summary>
    /// Downloads the current recording using the VideoManager instance.
    /// </summary>
    private void DownloadRecording()
    {
        VideoManager.Instance?.SaveCurrentRecording();
    }

    /// <summary>
    /// Cleans up resources and unregisters callbacks when the object is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        // Unregister all callbacks to prevent memory leaks
        if (_recordBtn != null)
        {
            _recordBtn.UnregisterCallback<ClickEvent>(evt => ToggleRecording());
        }

        if (_downloadBtn != null)
        {
            _downloadBtn.UnregisterCallback<ClickEvent>(evt => DownloadRecording());
        }

        // Clear references
        _recordBtn = null;
        _downloadBtn = null;       
    }

}