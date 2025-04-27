// Description: Manages persistent UI elements across scenes

using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.Localization.Settings;

/// <summary>
/// Singleton class to manage persistent UI elements across scenes,
/// ensuring localized text is applied after the localization system is ready.
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    // Persistent UI elements
    [SerializeField] private UIDocument _sharedUIDocument;

    private VisualElement _root;
    private VisualElement _navBar;
    private VisualElement _backToMenuButton;
    private VisualElement _collisionUI;
    private Button _playPauseBtn;
    private Button _restartBtn;
    private VisualElement _speedToggleButton;
    private Label _speedLabel;
    private Label _speedLeftArrow;
    private Label _speedRightArrow;

    private readonly float[] _timeScales = { 0.25f, 0.5f, 1f, 1.5f, 2f, 4f };
    private int _currentTimescaleIndex = 2;
    private VisualElement _recordBtn;
    private VisualElement _downloadBtn;

    private bool isPaused = false;
    private bool _isInitialized = false;


    private async void Awake()
    {
        // Ensure singleton instance
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // UI references
        _root = _sharedUIDocument.rootVisualElement;
        _navBar = _root.Q<VisualElement>("NavBar");
        _backToMenuButton = _root.Q<VisualElement>("BackToMenuButton");
        _collisionUI = _root.Q<VisualElement>("CollisionUI");

        InitializePersistentUI();
        InitializeCollisionUI();
        InitializeRecordButtons();

        // Delay localization-dependent setup
        await SetupAsync();
    }

    private async Task SetupAsync()
    {
        if (_isInitialized) return;

        // Wait for Unity's localization system
        await LocalizationSettings.InitializationOperation.Task;
        // Wait for any additional language loading
        if (LocalizationManager.Instance != null)
            await LocalizationManager.Instance.WaitForReady();

        // Load UIStrings table
        await LocalizedUIHelper.InitializeAsync();

        // Apply initial localized texts
        ApplyLocalization();
        LocalizationSettings.SelectedLocaleChanged += _ => ApplyLocalization();

        _isInitialized = true;
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        LocalizationSettings.SelectedLocaleChanged -= _ => ApplyLocalization();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        HideRecordButton();
        _collisionUI?.AddToClassList("d-none");
        isPaused = false;
        Time.timeScale = _timeScales[_currentTimescaleIndex];

        // Sjekk om den nye scenen er merket som kollisjonsscene
        if (GameObject.FindWithTag("CollisionScene") != null)
        {
            _collisionUI?.RemoveFromClassList("d-none");
            _playPauseBtn.text = "Pause";

            ShowRecordButton();
        }
    }

    /// <summary>
    /// Initializes the persistent UI elements
    /// </summary>
    private void InitializePersistentUI()
    {
        // Back-to-menu callback
        if (_backToMenuButton != null)
            _backToMenuButton.RegisterCallback<ClickEvent>(_ => OnBackToMainMenuClicked());

        HideNavBar();
    }

    private void InitializeCollisionUI()
    {
        if (_collisionUI == null) return;

        // Play / Restart
        _playPauseBtn  = _collisionUI.Q<Button>("playPauseButton");
        _restartBtn    = _collisionUI.Q<Button>("restartButton");

        _playPauseBtn.clicked += TogglePause;
        _restartBtn.clicked += RestartScene;

        // Speed toggle
        _speedToggleButton = _collisionUI.Q<VisualElement>("speedToggleButton");
        _speedLabel        = _speedToggleButton.Q<Label>("speedLabel");
        _speedLeftArrow    = _speedToggleButton.Q<Label>("speedLeftArrow");
        _speedRightArrow   = _speedToggleButton.Q<Label>("speedRightArrow");

        _speedLeftArrow.RegisterCallback<ClickEvent>(_ => DecreaseTimescale());
        _speedRightArrow.RegisterCallback<ClickEvent>(_ => IncreaseTimescale());
        _speedToggleButton.RegisterCallback<PointerDownEvent>(evt =>
        {
            if (evt.button == (int)MouseButton.MiddleMouse)
                ResetTimescale();
        });
    }

    private void ApplyLocalization()
    {
        // Play/Pause
        LocalizedUIHelper.Apply(_playPauseBtn, isPaused ? "Resume" : "Pause");
        // Restart
        LocalizedUIHelper.Apply(_restartBtn, "RestartButton");
        // Back-to-menu
        LocalizedUIHelper.Apply(_backToMenuButton.Q<Label>(), "MainMenu");
        // Speed label
        LocalizedUIHelper.Apply(_speedLabel, "Speed_Label");
        // Optionally arrows if they have keys, e.g. "<" / ">"
        LocalizedUIHelper.Apply(_speedLeftArrow, "SpeedLeftArrow");
        LocalizedUIHelper.Apply(_speedRightArrow, "SpeedRightArrow");

        // Update the numeric speed portion
        UpdateSpeedButtonText();
    }

    /// <summary>
    /// Initializes the record and download buttons in the NavBar and sets up theri click events
    /// The record button toggles recording state, and the download button saves the current recording
    /// </summary> 
    private void InitializeRecordButtons()
    {
        _recordBtn = _navBar.Q<VisualElement>("RecordButton2");
        if(_recordBtn != null)
        {
            var stopIcon = _recordBtn.Q<VisualElement>("StopIcon");
            var cameraIcon = _recordBtn.Q<VisualElement>("CameraIcon");
            stopIcon?.AddToClassList("d-none"); // Hide stop icon initially
            cameraIcon?.RemoveFromClassList("d-none"); // Show camera icon initially

            // Register the click event for the record button
            _recordBtn.RegisterCallback<ClickEvent>(evt => ToggleRecording());
        }

        _downloadBtn = _navBar.Q<VisualElement>("DownloadButton2");
        if(_downloadBtn != null) 
        {   
            Debug.Log("Download button found in NavBar");
            _downloadBtn.RegisterCallback<ClickEvent>(evt => DownloadRecording());
            // Hide the download button initially
            _downloadBtn.AddToClassList("d-none");
        }
    }

    /// <summary>
    /// Callback for the BackToMenuButton click event that loads the main menu scene
    /// </summary>
    private void OnBackToMainMenuClicked()
    {
        var label = _backToMenuButton?.Q<Label>();
        if (label == null) return;

        // Get localized key
        string currentKey = LocalizedUIHelper.GetKeyForText(label.text);
        Debug.Log($"Current key: {currentKey}");
        
        if (currentKey  == "MainMenu")
        {
            // Load the main menu scene
            Debug.Log("Loading MainMenu scene");
            SimulationManager.Instance.LoadScene("MainMenu");
        }
        else
        {
            // Go back to the previous scene
            Debug.Log($"Loading previous scene: {SimulationManager.Instance.PreviousScene}");
            SimulationManager.Instance.LoadScene(SimulationManager.Instance.PreviousScene);
        }
    }

    /// <summary>
    /// Shows the NavBar element
    /// </summary>
    public void ShowNavBar(string backButtonKey = "MainMenu")
    {
        _navBar.style.display = DisplayStyle.Flex;
        LocalizedUIHelper.Apply(_backToMenuButton.Q<Label>(), backButtonKey);
    }

    /// <summary>
    /// Hides the NavBar element
    /// </summary>
    public void HideNavBar()
    {
        _navBar.style.display = DisplayStyle.None;
    }

    /// <summary>
    /// Show the record button in the UI
    /// </summary>
    private void ShowRecordButton()
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
    private void HideRecordButton()
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
    public void UpdateRecordButton(string iconName, string buttonText)
    {
        if (_recordBtn != null)
        {
            var stopIcon = _recordBtn.Q<VisualElement>("StopIcon");
            var cameraIcon = _recordBtn.Q<VisualElement>("CameraIcon");

            if (iconName == "StopIcon")
            {
                stopIcon?.RemoveFromClassList("d-none");
                cameraIcon?.AddToClassList("d-none");
            }
            else
            {
                stopIcon?.AddToClassList("d-none");
                cameraIcon?.RemoveFromClassList("d-none");
            }

            // Update the text of the record button
            var label = _recordBtn.Q<Label>();
            if (label != null)
            {
                label.text = buttonText;
            }
        }
    }

    public void TogglePause()
    {
        // Toggle pause-status
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : _timeScales[_currentTimescaleIndex];
        LocalizedUIHelper.Apply(_playPauseBtn, isPaused ? "Resume" : "Pause");
    }

    public void RestartScene()
    {
        // Restart gjeldende scene
        isPaused = false;
        //Setter timescale lik slider
        Time.timeScale = _timeScales[_currentTimescaleIndex];
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.name);
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

    private void IncreaseTimescale()
    {
        _currentTimescaleIndex = (_currentTimescaleIndex + 1) % _timeScales.Length;
        Time.timeScale = _timeScales[_currentTimescaleIndex];
        UpdateSpeedButtonText();
    }

    private void DecreaseTimescale()
    {
        _currentTimescaleIndex--;
        if (_currentTimescaleIndex < 0) _currentTimescaleIndex = _timeScales.Length - 1;

        Time.timeScale = _timeScales[_currentTimescaleIndex];
        UpdateSpeedButtonText();
    }

    private void ResetTimescale()
    {
        _currentTimescaleIndex = System.Array.IndexOf(_timeScales, 1f);
        Time.timeScale = 1f;
        UpdateSpeedButtonText();

    }

    private void UpdateSpeedButtonText()
    {
        // "{0}x" portion comes from localization entry "Speed_Label"
        float scale = _timeScales[_currentTimescaleIndex];
        _speedLabel.text = string.Format(LocalizedUIHelper.Get("Speed_Label"), scale);
    }

    private void OnDestroy()
    {

        // Unregister the callback to avoid memory leaks
        if (_backToMenuButton != null)
        {
            _backToMenuButton.UnregisterCallback<ClickEvent>(e => OnBackToMainMenuClicked());
        }
        if(_recordBtn != null)
        {
            _recordBtn.UnregisterCallback<ClickEvent>(e => ToggleRecording());
        }
        if(_downloadBtn != null)
        {
            _downloadBtn.UnregisterCallback<ClickEvent>(e => DownloadRecording());
        }

        // Clean up references to avoid memory leaks
        Instance = null;
        _root = null;
        _navBar = null;
        _backToMenuButton = null;
        _sharedUIDocument = null;
        _recordBtn = null;
        _downloadBtn = null;
    }


}