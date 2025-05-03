using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class CollisionUIController : MonoBehaviour
{

    private Button _playPauseBtn;
    private Button _restartBtn;
    private VisualElement _speedToggleButton;
    private Label _speedLabel;
    private VisualElement _speedLeftArrow;
    private VisualElement _speedRightArrow;
    private bool _isPaused = false;
    private readonly float[] _timeScales = { 0.25f, 0.5f, 1f, 1.5f, 2f, 4f };
    private int _currentTimescaleIndex = 2;


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
        _speedToggleButton = UIManager.Instance.GetElement<VisualElement>("speedToggleButton");
        _speedLabel = UIManager.Instance.GetElement<Label>("speedLabel");
        _speedLeftArrow = UIManager.Instance.GetElement<VisualElement>("speedLeftArrow");
        _speedRightArrow = UIManager.Instance.GetElement<VisualElement>("speedRightArrow");
        
        if(_speedLeftArrow != null)
        {
            _speedLeftArrow.RegisterCallback<ClickEvent>(_ => DecreaseTimescale());
        }

        if(_speedRightArrow != null)
        {
            _speedRightArrow.RegisterCallback<ClickEvent>(_ => IncreaseTimescale());
        }

        if(_speedToggleButton != null)
        {
            _speedToggleButton.RegisterCallback<PointerDownEvent>(evt =>
            {
                if (evt.button == (int)MouseButton.MiddleMouse)
                    ResetTimescale();
            });
        }

    }

    private void ResetScene()
    {
        _isPaused = false;
        ResetTimescale();
     
        // Hide play icon initially and show pause icon
        _playPauseBtn?.Q<VisualElement>("playIcon")?.AddToClassList("d-none");
        _playPauseBtn?.Q<VisualElement>("pauseIcon")?.RemoveFromClassList("d-none");
    }

    private void InitializeCollisionScene()
    {
        ResetScene();
        _playPauseBtn.text = "Pause";
    }

    public void RestartScene()
    {
        _isPaused = false;
        // Reset the time scale to normal
        Time.timeScale = _timeScales[_currentTimescaleIndex];;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void TogglePause()
    {
        // Toggle pause-status
        _isPaused = !_isPaused;
        Time.timeScale = _isPaused ? 0f : _timeScales[_currentTimescaleIndex];
        LocalizedUIHelper.Apply(_playPauseBtn, _isPaused ? "Resume" : "Pause");

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

    public void IncreaseTimescale()
    {
        _currentTimescaleIndex = (_currentTimescaleIndex + 1) % _timeScales.Length;
        Time.timeScale = _timeScales[_currentTimescaleIndex];
        UpdateSpeedButtonText();
    }

    public void DecreaseTimescale()
    {
        _currentTimescaleIndex--;
        if (_currentTimescaleIndex < 0) _currentTimescaleIndex = _timeScales.Length - 1;

        Time.timeScale = _timeScales[_currentTimescaleIndex];
        UpdateSpeedButtonText();
    }

    public void ResetTimescale()
    {
        _currentTimescaleIndex = System.Array.IndexOf(_timeScales, 1f);
        Time.timeScale = 1f;
        UpdateSpeedButtonText();

        Debug.Log("[CollisionUIController] Timescale reset to 1.0");
    }

    private void UpdateSpeedButtonText()
    {
        // "{0}x" portion comes from localization entry "Speed_Label"
        float scale = _timeScales[_currentTimescaleIndex];
        _speedLabel.text = string.Format(LocalizedUIHelper.Get("Speed_Label"), scale);
    }

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
        _speedToggleButton = null;
        _speedLabel = null;
        _speedLeftArrow = null;
        _speedRightArrow = null;
    }
}