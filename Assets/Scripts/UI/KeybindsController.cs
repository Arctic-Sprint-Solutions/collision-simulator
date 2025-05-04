using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

/// <summary>
/// KeybindsController manages the keybinds UI panel in the collision scenes.
/// It initializes the UI, populates the keybinds menu with localized descriptions,
/// and handles the visibility of the keybinds panel.
/// </summary>
public class KeybindsController : MonoBehaviour
{
    [SerializeField] private KeybindSettings _keybindSettings;
    private VisualElement _keybindsEdge;
    private ScrollView _keybindsPanel;
    private VisualElement _toggleKeybindsPanel;
    private Label _showKeybindsLabel;
    private Label _hideKeybindsLabel;

    /// <summary>
    /// Initializes the KeybindsController and registers UIManager events.
    /// </summary>
    private void Start()
    {
        if (UIManager.Instance != null)
        {
            // Register UIManager events
            UIManager.Instance.OnCollisionSceneLoaded += InitializeCollisionScene;
            UIManager.Instance.OnNonCollisionSceneLoaded += InitializeNonCollisionScene;

            InitializeUI();
            Debug.Log("[KeybindsController] UI initialized successfully.");
        }
    }

    /// <summary>
    /// Cleans up the KeybindsController by unregistering UIManager events.
    /// </summary>
    private void OnDisable()
    {
        if (UIManager.Instance != null)
        {
            // Unregister UIManager events
            UIManager.Instance.OnCollisionSceneLoaded -= InitializeCollisionScene;
            UIManager.Instance.OnNonCollisionSceneLoaded -= InitializeNonCollisionScene;
        }
    }

    /// <summary>
    /// Initializes the UI elements for the keybinds panel.
    /// This method retrieves the necessary UI elements from the UIManager and sets up the initial state.
    /// </summary>
    private void InitializeUI()
    {
        // Get the keybinds edge and panel from the UIManager
        _keybindsEdge = UIManager.Instance.GetElement<VisualElement>("KeybindsEdge");
        _keybindsPanel = UIManager.Instance.GetElement<ScrollView>("KeybindsPanel");
        _toggleKeybindsPanel = UIManager.Instance.GetElement<VisualElement>("ToggleKeybindsPanel");
        _showKeybindsLabel = UIManager.Instance.GetElement<Label>("ShowKeybinds");
        _hideKeybindsLabel = UIManager.Instance.GetElement<Label>("HideKeybinds");

        // Hide the keybinds panel initially
        _keybindsPanel?.RemoveFromClassList("visible");
        _showKeybindsLabel?.RemoveFromClassList("d-none");
        _hideKeybindsLabel?.AddToClassList("d-none");

        // Register the toggle keybinds panel click event
        _toggleKeybindsPanel.RegisterCallback<ClickEvent>(evt => ToggleKeybindsPanel());
    }

    /// <summary>
    /// Populates the keybinds menu with localized descriptions and current keybinds.
    /// This method retrieves keybinds from the KeybindSettings ScriptableObject and displays them in the UI.
    /// </summary>
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
            var keyCode = (KeyCode)fieldInfo.GetValue(_keybindSettings);

            // fetch the localized description string
            string description = LocalizedUIHelper.Get(key.Value);

            // create a Label: “<Localized-description>: <KeyCode>”
            var row = new Label($"{description}: {keyCode}");
            row.AddToClassList("keybind-row");

            _keybindsPanel.contentContainer.Add(row);
        }
    }

    /// <summary>
    /// Initializes the keybinds panel for collision scenes.
    /// This method ensures the keybinds edge is visible and hides the keybinds panel initially.
    /// </summary>
    private void InitializeCollisionScene()
    {
        if(_keybindsEdge != null)
        {
            _keybindsEdge.RemoveFromClassList("d-none");
        }

        HideKeybindsPanel();
    }

    /// <summary>
    /// Initializes the keybinds panel for non-collision scenes.
    /// This method hides the keybinds edge and ensures the keybinds panel is not visible.
    /// </summary>
    private void InitializeNonCollisionScene()
    {
        if(_keybindsEdge != null)
        {
            _keybindsEdge.AddToClassList("d-none");
        }

        HideKeybindsPanel();
    }

    /// <summary>
    /// Hides the keybinds panel and updates the visibility labels.
    /// </summary>
    public void HideKeybindsPanel()
    {
        if (_keybindsPanel != null)
        {
            _keybindsPanel.RemoveFromClassList("visible");
            _showKeybindsLabel.RemoveFromClassList("d-none");
            _hideKeybindsLabel.AddToClassList("d-none");
        }
    }

    /// <summary>
    /// Shows the keybinds panel and updates the visibility labels.
    /// </summary>
    public void ShowKeybindsPanel()
    {
        if (_keybindsPanel != null)
        {
            _keybindsPanel.AddToClassList("visible");
            _showKeybindsLabel.AddToClassList("d-none");
            _hideKeybindsLabel.RemoveFromClassList("d-none");
        }
    }

    /// <summary>
    /// Toggles the visibility of the keybinds panel.
    /// If the panel is visible, it hides it; otherwise, it populates the menu and shows it.
    /// </summary>
    public void ToggleKeybindsPanel()
    {
        if (_keybindsPanel.ClassListContains("visible"))
        {
            HideKeybindsPanel();
        }
        else
        {
            PopulateKeybindsMenu();
            ShowKeybindsPanel();
        }
    }

    /// <summary>
    /// Clean up resources when the object is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        // Unregister the toggle keybinds panel click event
        if (_toggleKeybindsPanel != null)
        {
            _toggleKeybindsPanel.UnregisterCallback<ClickEvent>(evt => ToggleKeybindsPanel());
        }

        // Remove references to avoid memory leaks
        _keybindsEdge = null;
        _keybindsPanel = null;
        _toggleKeybindsPanel = null;
        _showKeybindsLabel = null;
        _hideKeybindsLabel = null;
        _keybindSettings = null;
    }
}