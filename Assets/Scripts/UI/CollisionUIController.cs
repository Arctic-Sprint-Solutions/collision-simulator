using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

/// <summary>
/// CollisionUIController manages the controls and UI elements for the collision scene.
/// It handles play/pause functionality, scene restarting, and time scale adjustments.
/// It listens to events from UIManager and InputManager to update the UI and respond to user inputs.
/// </summary>
public class CollisionUIController : MonoBehaviour
{
    private Button _playPauseBtn;
    private Button _restartBtn;
    private Label _speedLabel;
    private VisualElement _speedLeftArrow;
    private VisualElement _speedRightArrow;
    private bool _isPaused = false;
    private readonly float[] _timeScales = { 0.25f, 0.5f, 1f, 1.5f, 2f, 4f };
    private int _currentTimescaleIndex = 2;

    /// <summary>
    /// Initializes the CollisionUIController, setting up event listeners and UI elements.
    /// </summary>
    private void Start()
    {
        if(UIManager.Instance != null) 
        {
            // Register UIManager events
            UIManager.Instance.OnCollisionSceneLoaded += InitializeCollisionScene;
            UIManager.Instance.OnNonCollisionSceneLoaded += ResetScene;
            
            InitializeUI();
            Debug.Log("[CollisionUIControlle] UI initialized successfully.");
        }

        if(InputManager.Instance != null)
        {
            // Register InputManager events
            InputManager.Instance.OnTogglePause += TogglePause;
            InputManager.Instance.OnRestartScene += RestartScene;
            InputManager.Instance.OnIncreaseTimescale += IncreaseTimescale;
            InputManager.Instance.OnDecreaseTimescale += DecreaseTimescale;
            InputManager.Instance.OnResetTimescale += ResetTimescale;

            Debug.Log("[CollisionUIController] InputManager events registered successfully.");
        }
    }

    /// <summary>
    /// Initializes the UI elements and sets up event listeners for buttons and speed controls.
    /// </summary>
    private void InitializeUI()
    {
        // Get the play/pause and restart buttons from the UIManager
        _playPauseBtn = UIManager.Instance.GetElement<Button>("playPauseButton");
        _restartBtn = UIManager.Instance.GetElement<Button>("restartButton");

        if(_playPauseBtn != null)
        {
            _playPauseBtn.RemoveFromClassList("unity-button");
            _playPauseBtn.clicked += TogglePause;
            // Hide play icon initially
            _playPauseBtn.Q<VisualElement>("playIcon")?.AddToClassList("d-none");
        }

        if(_restartBtn != null)
        {
            _restartBtn.RemoveFromClassList("unity-button");
            _restartBtn.clicked += RestartScene;
        }

        // Get the speed control elements from the UIManager
        _speedLabel = UIManager.Instance.GetElement<Label>("speedLabel");
        _speedLeftArrow = UIManager.Instance.GetElement<VisualElement>("speedLeftArrow");
        _speedRightArrow = UIManager.Instance.GetElement<VisualElement>("speedRightArrow");
        
        if(_speedLeftArrow != null)
        {
            // Register click event for the left arrow to decrease timescale
            _speedLeftArrow.RegisterCallback<ClickEvent>(_ => DecreaseTimescale());
        }

        if(_speedRightArrow != null)
        {
            // Register click event for the right arrow to increase timescale
            _speedRightArrow.RegisterCallback<ClickEvent>(_ => IncreaseTimescale());
        }
    }

    /// <summary>
    /// Rests the scene to its initial state.
    /// </summary>
    private void ResetScene()
    {
        _isPaused = false;
        ResetTimescale();
     
        // Hide play icon initially and show pause icon
        _playPauseBtn?.Q<VisualElement>("playIcon")?.AddToClassList("d-none");
        _playPauseBtn?.Q<VisualElement>("pauseIcon")?.RemoveFromClassList("d-none");
    }

    /// <summary>
    /// Initializes the collision scene by resetting the scene state.
    /// </summary>
    private void InitializeCollisionScene()
    {
        ResetScene();
    }

    /// <summary>
    /// Restarts the current scene, resetting the time scale and pause state.
    /// </summary>
    public void RestartScene()
    {
        _isPaused = false;
        // Reset the time scale to normal
        Time.timeScale = _timeScales[_currentTimescaleIndex];;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Toggles the pause state of the simulation.
    /// When paused, the time scale is set to 0, and the play icon is shown.
    /// When unpaused, the time scale is set to the current timescale.
    /// </summary>
    public void TogglePause()
    {
        // Toggle pause-status
        _isPaused = !_isPaused;
        Time.timeScale = _isPaused ? 0f : _timeScales[_currentTimescaleIndex];

        if(_isPaused)
        {
            // Show the play icon and hide the pause icon when paused
            _playPauseBtn.Q<VisualElement>("playIcon")?.RemoveFromClassList("d-none");
            _playPauseBtn.Q<VisualElement>("pauseIcon")?.AddToClassList("d-none");
        }
        else
        {
            // Hide the play icon and show the pause icon when not paused
            _playPauseBtn.Q<VisualElement>("playIcon")?.AddToClassList("d-none");
            _playPauseBtn.Q<VisualElement>("pauseIcon")?.RemoveFromClassList("d-none");
        }
    }

    /// <summary>
    /// Increases the timescale by one step, cycling back to the first element if at the end.
    /// </summary>
    public void IncreaseTimescale()
    {
        _currentTimescaleIndex = (_currentTimescaleIndex + 1) % _timeScales.Length;
        Time.timeScale = _timeScales[_currentTimescaleIndex];
        UpdateSpeedButtonText();
    }

    /// <summary>
    /// Decreases the timescale by one step, cycling back to the last element if at the beginning.
    /// </summary>
    public void DecreaseTimescale()
    {
        _currentTimescaleIndex--;
        if (_currentTimescaleIndex < 0) _currentTimescaleIndex = _timeScales.Length - 1;

        Time.timeScale = _timeScales[_currentTimescaleIndex];
        UpdateSpeedButtonText();
    }

    /// <summary>
    /// Resets the timescale to 1.0 and updates the UI accordingly.
    /// </summary>
    public void ResetTimescale()
    {
        _currentTimescaleIndex = System.Array.IndexOf(_timeScales, 1f);
        Time.timeScale = 1f;
        UpdateSpeedButtonText();

        Debug.Log("[CollisionUIController] Timescale reset to 1.0");
    }

    /// <summary>
    /// Updates the speed button text to reflect the current time scale.
    /// </summary>
    private void UpdateSpeedButtonText()
    {
        // "{0}x" portion comes from localization entry "Speed_Label"
        float scale = _timeScales[_currentTimescaleIndex];
        _speedLabel.text = string.Format(LocalizedUIHelper.Get("Speed_Label"), scale);
    }

    /// <summary>
    /// Called when the object is destroyed to clean up event subscriptions and references.
    /// </summary>
    private void OnDestroy()
    {
        // Unregister all events to prevent memory leaks
        if(UIManager.Instance != null) 
        {
            UIManager.Instance.OnCollisionSceneLoaded -= InitializeCollisionScene;
            UIManager.Instance.OnNonCollisionSceneLoaded -= ResetScene;
        }

        if(InputManager.Instance != null)
        {
            InputManager.Instance.OnTogglePause -= TogglePause;
            InputManager.Instance.OnRestartScene -= RestartScene;
            InputManager.Instance.OnIncreaseTimescale -= IncreaseTimescale;
            InputManager.Instance.OnDecreaseTimescale -= DecreaseTimescale;
            InputManager.Instance.OnResetTimescale -= ResetTimescale;
        }

        // Clear references to UI elements
        _playPauseBtn = null;
        _restartBtn = null;
        _speedLabel = null;
        _speedLeftArrow = null;
        _speedRightArrow = null;
    }
}