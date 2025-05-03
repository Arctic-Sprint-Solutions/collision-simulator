// Description: Manages persistent UI elements across scenes

using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.Localization.Settings;
using System.Collections.Generic;

/// <summary>
/// Singleton class to manage persistent UI elements across scenes,
/// ensuring localized text is applied after the localization system is ready.
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    
    // Persistent UI elements
    [SerializeField] private UIDocument _sharedUIDocument;
    [SerializeField] private KeybindSettings _keybindSettings;
    private Dictionary<string, VisualElement> _uiElements = new Dictionary<string, VisualElement>();

    private VisualElement _root;
    private VisualElement _rootContainer;
    private VisualElement _navBar;
    private VisualElement _backToMenuButton;
    private VisualElement _collisionUI;
    private DropdownField _cameraDropdown;
    private VisualElement _cameraDropdownUI;
    private VisualElement _keybindsEdge;
    private ScrollView _keybindsPanel;
    private VisualElement _toggleKeybindsPanel;
    private Label _showKeybindsLabel;
    private Label _hideKeybindsLabel;
    private bool _isInitialized = false;
    private bool _isZenMode = false;

       
    public DropdownField CameraDropdown
    {
        get => _cameraDropdown;
        set => _cameraDropdown = value;
    }

    public VisualElement CameraDropdownUI
    {
        get => _cameraDropdownUI;
        set => _cameraDropdownUI = value;
    }

    private enum BackButtonMode { MainMenu, PreviousScene }
    private BackButtonMode _currentBackButtonMode;

    private List<UILocalizer> registeredLocalizers = new List<UILocalizer>();

    #region  Events
    public event System.Action OnCollisionSceneLoaded;
    public event System.Action OnNonCollisionSceneLoaded;
    public event System.Action OnApplyLocalization;
    #endregion

    /// <summary>
    /// Singleton instance of the UIManager
    /// </summary>
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
        _rootContainer = _root.Q<VisualElement>("Root");
        _navBar = _root.Q<VisualElement>("NavBar");
        _backToMenuButton = _root.Q<VisualElement>("BackToMenuButton");
        _keybindsEdge  = _root.Q<VisualElement>("KeybindsEdge");
        _keybindsPanel = _root.Q<ScrollView>("KeybindsPanel");
        _showKeybindsLabel = _root.Q<Label>("ShowKeybinds");
        _collisionUI = _root.Q<VisualElement>("CollisionUI");

        // Register for localization updates
        LocalizationManager.LocalizationUpdated += ApplyLocalization;

        // Initialize UI elements
        InitializePersistentUI();
        // InitializeCollisionUI();
        InitializeCameraDropDownUI();
        InitializeKeybindsHoverPanel();

        RegisterUIComponents();

        // Delay localization-dependent setup
        await SetupAsync();
    }

    private void RegisterUIComponents()
    {
        if(_collisionUI == null) return;
        // Collision UI components
        RegisterComponent("playPauseButton", _collisionUI.Q<Button>("playPauseButton"));
        RegisterComponent("restartButton", _collisionUI.Q<Button>("restartButton"));
        RegisterComponent("speedToggleButton", _collisionUI.Q<VisualElement>("speedToggleButton"));
        RegisterComponent("speedLabel", _collisionUI.Q<Label>("speedLabel"));
        RegisterComponent("speedLeftArrow", _collisionUI.Q<VisualElement>("speedLeftArrow"));
        RegisterComponent("speedRightArrow", _collisionUI.Q<VisualElement>("speedRightArrow"));

        // Recording components
        RegisterComponent("RecordButton", _collisionUI.Q<VisualElement>("RecordButton"));
        RegisterComponent("DownloadButton", _collisionUI.Q<VisualElement>("DownloadButton"));
    }

    private void RegisterComponent(string name, VisualElement element)
    {
        if(element == null)
        {
            Debug.LogWarning($"UIManager: Attempted to register a null element for '{name}'");
            return;
        }

        _uiElements[name] = element;
    }

    public T GetElement<T>(string name) where T : VisualElement
    {
        if (_uiElements.TryGetValue(name, out VisualElement element))
        {
            return element as T;
        }
        return null;
    }


    /// <summary>
    /// Sets up the UIManager by waiting for the localization system to be ready
    /// and applying initial localization.
    /// </summary>
    private async Task SetupAsync()
    {
        if (_isInitialized) return;

        // Wait for Unity's localization system
        await LocalizationSettings.InitializationOperation.Task;
        if (LocalizationManager.Instance != null)
            while (!LocalizationManager.Instance.IsLocalizationReady)
                await Task.Yield();

        // Load UIStrings table
        await LocalizedUIHelper.InitializeAsync();

        // Apply initial localized texts
        ApplyLocalization();

        _isInitialized = true;
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;

    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _collisionUI?.AddToClassList("d-none");
        _keybindsEdge?.AddToClassList("d-none");

        // Sjekk om den nye scenen er merket som kollisjonsscene
        if (GameObject.FindWithTag("CollisionScene") != null)
        {
            _collisionUI?.RemoveFromClassList("d-none");
            // _playPauseBtn.text = "Pause";
            _keybindsEdge?.RemoveFromClassList("d-none");

            // Notify that a collision scene has been loaded
            OnCollisionSceneLoaded?.Invoke();   
        } 
        else
        {
            // Notify that a non-collision scene has been loaded
            OnNonCollisionSceneLoaded?.Invoke();
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

    /// <summary>
    /// Initializes the UI elements for camera selection
    /// </summary>
    private void InitializeCameraDropDownUI()
    {
        if (_collisionUI == null) return;

        // Get the camera dropdown and make it visible
        _cameraDropdownUI = _collisionUI.Q<VisualElement>("CameraDropdownContainer");

        if (_cameraDropdownUI == null)
        {
            Debug.LogError("UIManager: cameraDropdownUI not found in the UI.");
            return;
        }

        _cameraDropdown = _cameraDropdownUI.Q<DropdownField>("CameraDropdown");
        if (_cameraDropdown == null)
        {
            Debug.LogError("CameraController: Camera DropdownField is null.");
            return;
        }
        
        Debug.Log("CameraController: Camera Dropdown UI initialized successfully.");

    }

    private void InitializeKeybindsHoverPanel()
    {
        if (_collisionUI == null) return;

        _keybindsEdge  = _root.Q<VisualElement>("KeybindsEdge");
        _keybindsPanel = _root.Q<ScrollView>("KeybindsPanel");
        
        if (_keybindsEdge == null || _keybindsPanel == null)
        {
            Debug.LogError("KeybindsEdge or KeybindsPanel not found.");
            return;
        }

        _showKeybindsLabel = _keybindsEdge.Q<Label>("ShowKeybinds");
        _hideKeybindsLabel = _keybindsEdge.Q<Label>("HideKeybinds");
        _toggleKeybindsPanel = _keybindsEdge.Q<VisualElement>("ToggleKeybindsPanel");

        // Hide the keybinds panel initially
        _keybindsPanel.RemoveFromClassList("visible");
        _showKeybindsLabel.RemoveFromClassList("d-none");
        _hideKeybindsLabel.AddToClassList("d-none");

        // Register the click event for the keybinds edge
        _toggleKeybindsPanel.RegisterCallback<ClickEvent>(evt =>
        {
            // Toggle visibility of the keybinds panel
            if (_keybindsPanel.ClassListContains("visible"))
            {
                _keybindsPanel.RemoveFromClassList("visible");
                _showKeybindsLabel.RemoveFromClassList("d-none");
                _hideKeybindsLabel.AddToClassList("d-none");
            }
            else
            {
                PopulateKeybindsMenu();
                _keybindsPanel.AddToClassList("visible");
                _showKeybindsLabel.AddToClassList("d-none");
                _hideKeybindsLabel.RemoveFromClassList("d-none");
            }
        });

    }

    private void PopulateKeybindsMenu()
    {
        _keybindsPanel.contentContainer.Clear();

        // map each ScriptableObject field → your Localization Table key
        var map = new Dictionary<string, string>
        {
            { nameof(_keybindSettings.pauseKey), "PauseKeybindLabel"},
            { nameof(_keybindSettings.restartKey), "RestartKeybindLabel"},
            { nameof(_keybindSettings.increaseSpeedKey), "IncreaseSpeedKeybindLabel" },
            { nameof(_keybindSettings.decreaseSpeedKey), "DecreaseSpeedKeybindLabel" },
            { nameof(_keybindSettings.resetSpeedKey), "ResetSpeedKeybindLabel" },
            { nameof(_keybindSettings.recordKey), "RecordKeybindLabel"},
            { nameof(_keybindSettings.saveKey), "DownloadKeybindLabel"},
            { nameof(_keybindSettings.zenModeKey), "ZenKeybindLabel" }
        };

        var settingsType = typeof(KeybindSettings);
        foreach (var key in map)
        {
            // get the KeyCode value via reflection
            var fieldInfo = settingsType.GetField(key.Key);
            var keyCode   = (KeyCode) fieldInfo.GetValue(_keybindSettings);

            // fetch the localized description string
            string description = LocalizedUIHelper.Get(key.Value);

            // create a Label: “<Localized-description>: <KeyCode>”
            var row = new Label($"{description}: {keyCode}");
            row.AddToClassList("keybind-row");

            _keybindsPanel.contentContainer.Add(row);
        }
    }

    ///<summary>
    /// Applies localization to UI elements
    /// </summary>
    /// <remarks>
    public void ApplyLocalization()
    {
        if (!_isInitialized || LocalizationManager.Instance == null || !LocalizationManager.Instance.IsLocalizationReady)
        {
            return;
        }

        Debug.Log("[UIManager] ApplyLocalization running...");

        // Make sure LocalizedUIHelper re-fetches the StringTable freshly
        LocalizedUIHelper.ReloadTable();

        if (_backToMenuButton != null)
        {
            var label = _backToMenuButton.Q<Label>();
            if (label != null)
            {
                string key = _currentBackButtonMode == BackButtonMode.PreviousScene ? "GoBack" : "MainMenu";
                LocalizedUIHelper.Apply(label, key);
            }
        }

        var speedLabel = GetElement<Label>("SpeedLabel");
        if(speedLabel != null)
        {
            LocalizedUIHelper.Apply(speedLabel, "Speed_Label");
        }

        if (_showKeybindsLabel != null)
        {
            var label = _showKeybindsLabel.Q<Label>("ShowKeybinds");
            if (label != null)
            {
                LocalizedUIHelper.Apply(label, "ShowKeybindsLabel");
            }
        }

        if (_hideKeybindsLabel != null)
        {
            var label = _hideKeybindsLabel.Q<Label>("HideKeybinds");
            if (label != null)
            {
                LocalizedUIHelper.Apply(label, "HideKeybindsLabel");
            }
        }

        // Tell all localizers to reload too
        foreach (var localizer in registeredLocalizers)
        {
            if (localizer != null)
                localizer.ReloadLocalization();
        }
    }

    /// <summary>
    /// Callback for the BackToMenuButton click event that loads the main menu scene
    /// </summary>
    private void OnBackToMainMenuClicked()
    {
        if (_currentBackButtonMode == BackButtonMode.PreviousScene)
            SimulationManager.Instance.LoadScene(SimulationManager.Instance.PreviousScene);
        else
            SimulationManager.Instance.LoadScene("MainMenu");
    }

    /// <summary>
    /// Shows the NavBar element
    /// </summary>
    public void ShowNavBar(bool goBack = false)
    {
        if (_navBar == null) return;
        _navBar.style.display = DisplayStyle.Flex;
        _currentBackButtonMode = goBack ? BackButtonMode.PreviousScene : BackButtonMode.MainMenu;
        ApplyLocalization();
    }

    /// <summary>
    /// Hides the NavBar element
    /// </summary>
    public void HideNavBar()
    {
        _navBar.style.display = DisplayStyle.None;
    }

    public void ToggleZenMode()
    {
        _isZenMode = !_isZenMode;
        if (_rootContainer == null) return;

        _rootContainer.style.display = _isZenMode ? DisplayStyle.None : DisplayStyle.Flex;
    }

    public void RegisterLocalizer(UILocalizer localizer)
    {
        if (!registeredLocalizers.Contains(localizer))
        {
            registeredLocalizers.Add(localizer);
        }
    }

    private void OnDestroy()
    {

        // Unregister the callback to avoid memory leaks
        if (_backToMenuButton != null)
        {
            _backToMenuButton.UnregisterCallback<ClickEvent>(e => OnBackToMainMenuClicked());
        }

        // Unregister localization update callback
        LocalizationManager.LocalizationUpdated -= ApplyLocalization;

        // Clean up references to avoid memory leaks
        Instance = null;
        _root = null;
        _navBar = null;
        _backToMenuButton = null;
        _sharedUIDocument = null;
    }

}