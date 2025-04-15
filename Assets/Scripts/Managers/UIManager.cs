// Description: Manages persistent UI elements across scenes

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

/// <summary>
/// Singleton class to manage persistent UI elements across scenes
/// </summary>
public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance => _instance;

    // Persistent UI elements
    [SerializeField] private UIDocument _sharedUIDocument;
    [SerializeField] private VisualTreeAsset _SceneUIDocument;

    private VisualElement _root;
    private VisualElement _navBar;
    private VisualElement _backToMenuButton;
    private VisualElement _collisionUI;
    private Button _playPauseBtn;
    private Button _restartBtn;
    private Slider _speedSlider;

    private bool isPaused = false;


    private void Awake()
    {
        // Ensure singleton instance
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        InitializePersistentUI();
    }

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
        _collisionUI?.AddToClassList("d-none");
        isPaused = false;
        Time.timeScale = 1f;

        // Sjekk om den nye scenen er merket som kollisjonsscene
        if (GameObject.FindWithTag("CollisionScene") != null)
        {
            _collisionUI?.RemoveFromClassList("d-none");
            _playPauseBtn.text = "Pause";
        }
    }

    /// <summary>
    /// Initializes the persistent UI elements
    /// </summary>
    private void InitializePersistentUI()
    {
        // Get the root visual element of the shared UI document
        _root = _sharedUIDocument.rootVisualElement;
        // Get the NavBar and BackToMenuButton elements
        if (_navBar == null)
        {
            // Get the NavBar element from the shared UI document
            _navBar = _root.Q<VisualElement>("NavBar");
        }

        if (_backToMenuButton == null)
        {
            // Get the BackToMenuButton element from the shared UI document
            _backToMenuButton = _root.Q<VisualElement>("BackToMenuButton");
            // Register a callback for the button's click event
            _backToMenuButton.RegisterCallback<ClickEvent>(e => OnBackToMainMenuClicked());
        }

        if (_collisionUI == null)
        {
            InitializeCollisionUI();
        }

        // Hide the NavBar by default
        HideNavBar();
    }

    private void InitializeCollisionUI()
    {
        _collisionUI = _root.Q<VisualElement>("CollisionUI");
        _collisionUI.RemoveFromClassList("d-none");
        // Finn knappene i det instansierte UI-et
        _speedSlider = _collisionUI.Q<Slider>("speedSlider");
        _playPauseBtn = _collisionUI.Q<Button>("playPauseButton");
        _restartBtn = _collisionUI.Q<Button>("restartButton");
        _playPauseBtn.text = "Pause";

        // Hide Unity default classes
        _playPauseBtn.RemoveFromClassList("unity-button");
        _playPauseBtn.RemoveFromClassList("unity-text-element");
        _restartBtn.RemoveFromClassList("unity-button");
        _restartBtn.RemoveFromClassList("unity-text-element");

        // Koble opp klikk-event til h�ndteringsfunksjoner
        _playPauseBtn.clicked += TogglePause;
        _restartBtn.clicked += RestartScene;

        // slider logikk
        _speedSlider.lowValue = 0.1f;  // 0.1x% hastighet
        _speedSlider.highValue = 10.0f; // 10x hastighet
        _speedSlider.value = 1f;       // normal hastighet (100%)

        OrbitalMovement.speedMultiplier = 1f;

        _speedSlider.RegisterValueChangedCallback(evt =>
        {
            OrbitalMovement.speedMultiplier = evt.newValue;
        });
    }

    /// <summary>
    /// Callback for the BackToMenuButton click event that loads the main menu scene
    /// </summary>
    private void OnBackToMainMenuClicked()
    {
        string buttonText = _backToMenuButton.Q<Label>().text;
        if (buttonText == "Go Back")
        {
            // Go back to the previous scene
            SimulationManager.Instance.LoadScene(SimulationManager.Instance.PreviousScene);
        }
        else
        {
            // Load the main menu scene
            SimulationManager.Instance.LoadScene("MainMenu");
        }
    }

    /// <summary>
    /// Shows the NavBar element
    /// </summary>
    public void ShowNavBar(string backButtonText = "Main Menu")
    {
        if (_navBar != null)
        {
            _navBar.style.display = DisplayStyle.Flex;

            // Set the text of the BackToMenuButton
            if (_backToMenuButton != null)
            {
                _backToMenuButton.Q<Label>().text = backButtonText;
            }
        }
    }

    /// <summary>
    /// Hides the NavBar element
    /// </summary>
    public void HideNavBar()
    {
        if (_navBar != null)
        {
            _navBar.style.display = DisplayStyle.None;
        }

    }

    private void TogglePause()
    {
        // Toggle pause-status
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;

        _playPauseBtn.text = isPaused ? "Resume" : "Pause";
    }

    private void RestartScene()
    {
        // Restart gjeldende scene
        isPaused = false;
        Time.timeScale = 1f;
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.name);
    }

    private void OnDestroy()
    {

        // Unregister the callback to avoid memory leaks
        if (_backToMenuButton != null)
        {
            _backToMenuButton.UnregisterCallback<ClickEvent>(e => OnBackToMainMenuClicked());
        }

        // Clean up references to avoid memory leaks
        _instance = null;
        _root = null;
        _navBar = null;
        _backToMenuButton = null;
        _sharedUIDocument = null;
    }


}