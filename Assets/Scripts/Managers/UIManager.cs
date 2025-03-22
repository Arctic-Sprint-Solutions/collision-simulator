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
    private VisualElement collisionUI;

    private bool isPaused = false;

    /// <summary>
    /// Initializes the singleton instance and ensures that it persists across scenes
    /// </summary>
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
        // Fjern tidligere UI hvis det finnes
        if (collisionUI != null)
        {
            collisionUI.RemoveFromHierarchy();
            collisionUI = null;
        }

        // Sjekk om den nye scenen er merket som kollisjonsscene
        if (GameObject.FindWithTag("CollisionScene") != null)
        {
            // Instansier og vis knappene fra UXML
            var root = _sharedUIDocument.rootVisualElement;
            collisionUI = _SceneUIDocument.Instantiate();
            root.Add(collisionUI);

            // Finn knappene i det instansierte UI-et
            Button playPauseBtn = collisionUI.Q<Button>("playPauseButton");
            Button restartBtn = collisionUI.Q<Button>("restartButton");

            // Koble opp klikk-event til håndteringsfunksjoner
            playPauseBtn.clicked += TogglePause;
            restartBtn.clicked += RestartScene;
        }
        // (Hvis scenen ikke er kollisjonsscene, gjør vi ingenting – knappene forblir fjernet/skjult)
    }

    /// <summary>
    /// Initializes the persistent UI elements
    /// </summary>
    private void InitializePersistentUI()
    {
        // Get the root visual element of the shared UI document
        _root = _sharedUIDocument.rootVisualElement;
        // Get the NavBar and BackToMenuButton elements
        if(_navBar == null)
        {
            // Get the NavBar element from the shared UI document
            _navBar = _root.Q<VisualElement>("NavBar");
        }

        if(_backToMenuButton == null)
        {
            // Get the BackToMenuButton element from the shared UI document
            _backToMenuButton = _root.Q<VisualElement>("BackToMenuButton");
            // Register a callback for the button's click event
            _backToMenuButton.RegisterCallback<ClickEvent>(e => OnBackToMainMenuClicked());
        }

        // Hide the NavBar by default
        HideNavBar();
    }

    /// <summary>
    /// Callback for the BackToMenuButton click event that loads the main menu scene
    /// </summary>
    private void OnBackToMainMenuClicked()
    {
        SimulationManager.Instance.LoadScene("MainMenu");
    }

    /// <summary>
    /// Shows the NavBar element
    /// </summary>
    public void ShowNavBar()
    {
        if(_navBar != null)
        {
            _navBar.style.display = DisplayStyle.Flex;
            _navBar.BringToFront();
        }

    }

    /// <summary>
    /// Hides the NavBar element
    /// </summary>
    public void HideNavBar()
    {
        if(_navBar != null)
        {
            _navBar.style.display = DisplayStyle.None;
        }

    }

    private void TogglePause()
    {
        // Toggle pause-status
        isPaused = !isPaused;
        // Sett simulering på pause eller kjør
        Time.timeScale = isPaused ? 0f : 1f;

        var playPauseBtn = collisionUI.Q<Button>("playPauseButton");
        playPauseBtn.text = isPaused ? "Resume" : "Pause";
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