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
    #region  Properties
    /// <summary>
    /// Singleton instance of the UIManager
    /// </summary>
    public static UIManager Instance { get; private set; }
    /// <summary>
    /// Reference to the shared UIDocument that contains the persistent UI elements.
    /// </summary>
    [SerializeField] private UIDocument _sharedUIDocument;
    /// <summary>
    /// Dictionary to hold references to UI elements by name for easy access.
    /// </summary>
    private Dictionary<string, VisualElement> _uiElements = new Dictionary<string, VisualElement>();
    /// <summary>
    /// Reference to the root VisualElement of the shared UIDocument.
    /// </summary>
    private VisualElement _root;
    /// <summary>
    /// Reference to the root container of the UI.
    /// </summary>
    private VisualElement _rootContainer;
    /// <summary>
    /// Reference to the navigation bar element.
    /// </summary>
    private VisualElement _navBar;
    /// <summary>
    /// Reference to the back-to-menu button element.
    /// </summary>
    private VisualElement _backToMenuButton;
    /// <summary>
    /// Reference to the collision UI element.
    /// </summary>
    private VisualElement _collisionUI;
    /// <summary>
    /// Flag to indicate if the UIManager has been initialized.
    /// </summary>
    private bool _isInitialized = false;
    /// <summary>
    /// Flag to indicate if Zen Mode is active.
    /// Zen Mode hides the root container and all its children.
    /// </summary>
    private bool _isZenMode = false;
    /// <summary>
    /// Enum to define the back button mode.
    /// MainMenu mode loads the main menu scene,
    /// PreviousScene mode loads the previous scene.
    /// </summary>
    private enum BackButtonMode { MainMenu, PreviousScene }
    /// <summary>
    /// Current mode of the back button.
    /// </summary>
    private BackButtonMode _currentBackButtonMode;
    /// <summary>
    /// List of registered UILocalizers to receive localization updates.
    /// </summary>
    private List<UILocalizer> registeredLocalizers = new List<UILocalizer>();
    #endregion

    /// <summary>
    /// Awake method to initialize the UIManager.
    /// It sets up the singleton instance, registers UI components, and applies localization.
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

        // Initialize UI elements
        InitializePersistentUI();
        RegisterUIComponents();

        // Register for localization updates
        LocalizationManager.LocalizationUpdated += ApplyLocalization;

        // Delay localization-dependent setup
        await SetupAsync();
    }

    /// <summary>
    /// Registers UI components from the shared UIDocument.
    /// </summary>
    private void RegisterUIComponents()
    {
        if (_collisionUI == null) return;

        // Collision UI components
        RegisterComponent("playPauseButton", _collisionUI.Q<Button>("playPauseButton"));
        RegisterComponent("restartButton", _collisionUI.Q<Button>("restartButton"));
        RegisterComponent("speedLabel", _collisionUI.Q<Label>("speedLabel"));
        RegisterComponent("speedLeftArrow", _collisionUI.Q<VisualElement>("speedLeftArrow"));
        RegisterComponent("speedRightArrow", _collisionUI.Q<VisualElement>("speedRightArrow"));

        // Recording components
        RegisterComponent("RecordButton", _collisionUI.Q<VisualElement>("RecordButton"));
        RegisterComponent("DownloadButton", _collisionUI.Q<VisualElement>("DownloadButton"));
        RegisterComponent("RecordingFinishedLabel", _collisionUI.Q<Label>("RecordingFinishedLabel"));

        // Camera components
        RegisterComponent("CameraDropdownContainer", _collisionUI.Q<VisualElement>("CameraDropdownContainer"));
        RegisterComponent("CameraDropdown", _collisionUI.Q<DropdownField>("CameraDropdown"));

        if (_root == null) return;

        // Keybinds components
        RegisterComponent("KeybindsEdge", _root.Q<VisualElement>("KeybindsEdge"));
        RegisterComponent("KeybindsPanel", _root.Q<ScrollView>("KeybindsPanel"));
        RegisterComponent("ToggleKeybindsPanel", _root.Q<VisualElement>("ToggleKeybindsPanel"));
        RegisterComponent("ShowKeybinds", _root.Q<Label>("ShowKeybinds"));
        RegisterComponent("HideKeybinds", _root.Q<Label>("HideKeybinds"));

    }

    /// <summary>
    /// Registers a UI component by name and element and adds it to the dictionary.
    /// </summary>
    private void RegisterComponent(string name, VisualElement element)
    {
        if (element == null)
        {
            Debug.LogWarning($"UIManager: Attempted to register a null element for '{name}'");
            return;
        }

        // Add the element to the dictionary
        _uiElements[name] = element;
    }

    /// <summary>
    /// Retrieves a UI element by name and casts it to the specified type.
    /// </summary>
    /// <typeparam name="T">The type to cast the UI element to.</typeparam>
    /// <param name="name">The name of the UI element to retrieve.</param>
    /// <returns>The UI element cast to the specified type, or null if not found.</returns>
    public T GetElement<T>(string name) where T : VisualElement
    {
        // Check if the element exists in the dictionary and cast it to the specified type
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

    /// <summary>
    /// Initializes the persistent UI elements
    /// </summary>
    private void InitializePersistentUI()
    {
        // UI references
        _root = _sharedUIDocument.rootVisualElement;
        _rootContainer = _root.Q<VisualElement>("Root");
        _navBar = _root.Q<VisualElement>("NavBar");
        _backToMenuButton = _root.Q<VisualElement>("BackToMenuButton");
        _collisionUI = _root.Q<VisualElement>("CollisionUI");

        // Back-to-menu callback
        if (_backToMenuButton != null)
        {
            _backToMenuButton.RegisterCallback<ClickEvent>(_ => OnBackToMainMenuClicked());
        }

        HideNavBar();
    }

    ///<summary>
    /// Applies localization to UI elements
    /// </summary>
    public void ApplyLocalization()
    {
        // Check if the localization system is ready
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

        // Tell all localizers to reload too
        foreach (var localizer in registeredLocalizers)
        {
            if (localizer != null)
            {
                localizer.ReloadLocalization();
            }
        }
    }

    /// <summary>
    /// Callback for the BackToMenuButton click event that loads the main menu scene
    /// </summary>
    private void OnBackToMainMenuClicked()
    {
        if (_currentBackButtonMode == BackButtonMode.PreviousScene)
        {
            SimulationManager.Instance.LoadScene(SimulationManager.Instance.PreviousScene);
        }
        else
        {
            SimulationManager.Instance.LoadScene("MainMenu");
        }
    }

    /// <summary>
    /// Shows the NavBar element
    /// </summary>
    /// <param name="goBack">If true, sets the back button to go back to the previous scene; otherwise, it goes to the main menu.</param>
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

    /// <summary>
    /// Shows the Collision UI element
    /// </summary>
    public void ShowCollisionUI()
    {
        if (_collisionUI == null) return;

        _collisionUI.RemoveFromClassList("d-none");
    }

    /// <summary>
    /// Hides the Collision UI element
    /// </summary>
    public void HideCollisionUI()
    {
        if (_collisionUI == null) return;

        _collisionUI.AddToClassList("d-none");
    }

    /// <summary>
    /// Toggles the Zen Mode, which hides the root container and all its children
    /// </summary>
    public void ToggleZenMode()
    {
        _isZenMode = !_isZenMode;
        if (_rootContainer == null) return;

        _rootContainer.style.display = _isZenMode ? DisplayStyle.None : DisplayStyle.Flex;
    }

    /// <summary>
    /// Registers a UILocalizer to receive localization updates.
    /// </summary>
    /// <param name="localizer">The UILocalizer instance to register.</param>
    public void RegisterLocalizer(UILocalizer localizer)
    {
        if (!registeredLocalizers.Contains(localizer))
        {
            registeredLocalizers.Add(localizer);
        }
    }

    /// <summary>
    /// Unregisters a UILocalizer to stop receiving localization updates.
    /// </summary>
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
        _collisionUI = null;
        _uiElements.Clear();
        registeredLocalizers.Clear();
    }

}