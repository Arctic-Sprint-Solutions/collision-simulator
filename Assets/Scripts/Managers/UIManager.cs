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
        // Back-to-menu callback
        if (_backToMenuButton != null)
            _backToMenuButton.RegisterCallback<ClickEvent>(_ => OnBackToMainMenuClicked());

        // Collision UI buttons
        if (_collisionUI != null)
        {
            _playPauseBtn = _collisionUI.Q<Button>("playPauseButton");
            _restartBtn = _collisionUI.Q<Button>("restartButton");
            _playPauseBtn.text = "Pause";
            _playPauseBtn.clicked += TogglePause;
            _restartBtn.clicked += RestartScene;
        }

        HideNavBar();
    }

    private void InitializeCollisionUI()
    {
        _collisionUI = _root.Q<VisualElement>("CollisionUI");
        _collisionUI.RemoveFromClassList("d-none");
        // Finn knappene i det instansierte UI-et
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
    }

    private void ApplyLocalization()
    {
        // Localize play/pause
        if (_playPauseBtn != null)
        {
            LocalizedUIHelper.Apply(_playPauseBtn, isPaused ? "Resume" : "Pause");
        }
        if (_restartBtn != null)
        {
            LocalizedUIHelper.Apply(_restartBtn, "RestartButton");
        }
        if (_backToMenuButton != null)
        {
            var label = _backToMenuButton.Q<Label>();
            if (label != null)
                LocalizedUIHelper.Apply(label, "MainMenu"); // KEY must match exactly
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
        if (currentKey  == "Go Back")
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
    public void ShowNavBar(string backButtonKey = "MainMenu")
    {
        if (_navBar != null)
        {
            _navBar.style.display = DisplayStyle.Flex;

            // Set the text of the BackToMenuButton
            if (_backToMenuButton != null)
            {
                var label = _backToMenuButton.Q<Label>();
                if (label != null)
                    LocalizedUIHelper.Apply(label, backButtonKey);
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

    public void TogglePause()
    {
        // Toggle pause-status
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;

        if (_playPauseBtn != null)
        {
            LocalizedUIHelper.Apply(_playPauseBtn, isPaused ? "Resume" : "Pause");
        }
    }

    public void RestartScene()
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
        Instance = null;
        _root = null;
        _navBar = null;
        _backToMenuButton = null;
        _sharedUIDocument = null;
    }


}