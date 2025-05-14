// Description: Handles the settings page logic for binding keys and translating the application.

using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Localization.Settings;

/// <summary>
/// Controller for the settings page, handling key rebinding and language selection.
/// </summary>
public class SettingsController : MonoBehaviour
{
    #region Properties
    /// <summary>
    /// Reference to the UIDocument component for accessing UI elements.
    /// </summary>
    public UIDocument uiDocument;
    /// <summary>
    /// Dictionary mapping action names to their corresponding buttons.
    /// </summary>
    private Dictionary<string, Button> actionButtonMap;
    /// <summary>
    /// Dictionary to hold the key mappings for getting the key bindings.
    /// </summary>
    private Dictionary<string, Func<KeyCode>> getKeyMap;
    /// <summary>
    /// Dictionary to hold the key mappings for setting the key bindings.
    /// </summary>
    private Dictionary<string, Action<KeyCode>> setKeyMap;
    /// <summary>
    /// Reference to the dropdown field for language selection in the UI.
    /// </summary>
    private DropdownField languageDropdown;
    /// <summary>
    /// Indicates whether the controller is currently waiting for a key input.
    /// Defaults to false.
    /// </summary>
    private bool waitingForKey = false;
    /// <summary>
    /// Action to be invoked when a key is selected.
    /// </summary>
    private Action<KeyCode> onKeySelected;
    /// <summary>
    /// Coroutine for blinking the button text while waiting for a key input.
    /// Used to imitate a cursor line.
    /// </summary>
    private Coroutine blinkCoroutine;
    #endregion

    /// <summary>
    /// Calls the StartCoroutine to set up the settings controller.
    /// This method is called when the script instance is being loaded.
    /// </summary>
    private void Start()
    {
        StartCoroutine(SetupAsync());
    }

    /// <summary>
    /// Sets up the settings controller, waiting for the localization system to be ready.
    /// </summary>
    private IEnumerator SetupAsync()
    {
        yield return new WaitUntil(() => InputManager.Instance != null && InputManager.Instance.keybinds != null);
        yield return LocalizationSettings.InitializationOperation;
        if (LocalizationManager.Instance != null)
            yield return new WaitUntil(() => LocalizationManager.Instance.IsLocalizationReady);

        InitializeMappings();
        InitializeUI();
    }

    /// <summary>
    /// Initializes the key mappings for getting and setting key bindings.
    /// </summary>
    private void InitializeMappings()
    {
        getKeyMap = new Dictionary<string, Func<KeyCode>>()
        {
            { "Pause",         () => InputManager.Instance.keybinds.pauseKey },
            { "Restart",       () => InputManager.Instance.keybinds.restartKey },
            { "Record",        () => InputManager.Instance.keybinds.recordKey },
            { "Download",      () => InputManager.Instance.keybinds.saveKey },
            { "IncreaseSpeed", () => InputManager.Instance.keybinds.increaseSpeedKey },
            { "DecreaseSpeed", () => InputManager.Instance.keybinds.decreaseSpeedKey },
            { "ResetSpeed",    () => InputManager.Instance.keybinds.resetSpeedKey },
            { "ZenMode",       () => InputManager.Instance.keybinds.zenModeKey },
            { "Camera1",       () => InputManager.Instance.keybinds.camera1Key },
            { "Camera2",       () => InputManager.Instance.keybinds.camera2Key },
            { "Camera3",       () => InputManager.Instance.keybinds.camera3Key },
            { "ToggleKeybindPanel", () => InputManager.Instance.keybinds.toggleKeybindPanel }
        };

        setKeyMap = new Dictionary<string, Action<KeyCode>>()
        {
            { "Pause",         (key) => InputManager.Instance.keybinds.pauseKey = key },
            { "Restart",       (key) => InputManager.Instance.keybinds.restartKey = key },
            { "Record",        (key) => InputManager.Instance.keybinds.recordKey = key },
            { "Download",      (key) => InputManager.Instance.keybinds.saveKey = key },
            { "IncreaseSpeed", (key) => InputManager.Instance.keybinds.increaseSpeedKey = key },
            { "DecreaseSpeed", (key) => InputManager.Instance.keybinds.decreaseSpeedKey = key },
            { "ResetSpeed",    (key) => InputManager.Instance.keybinds.resetSpeedKey = key },
            { "ZenMode",       (key) => InputManager.Instance.keybinds.zenModeKey = key },
            { "Camera1",       (key) => InputManager.Instance.keybinds.camera1Key = key },
            { "Camera2",       (key) => InputManager.Instance.keybinds.camera2Key = key },
            { "Camera3",       (key) => InputManager.Instance.keybinds.camera3Key = key },
            { "ToggleKeybindPanel", (key) => InputManager.Instance.keybinds.toggleKeybindPanel = key }

        };
    }

    /// <summary>
    /// Initializes the UI and sets up the key binding buttons.
    /// </summary>
    private void InitializeUI()
    {
        var root = uiDocument.rootVisualElement;

        actionButtonMap = new Dictionary<string, Button>
        {
            { "Pause",         root.Q<Button>("PauseKeybindButton") },
            { "Restart",       root.Q<Button>("RestartKeybindButton") },
            { "Record",        root.Q<Button>("RecordKeybindButton") },
            { "Download",      root.Q<Button>("DownloadKeybindButton") },
            { "IncreaseSpeed", root.Q<Button>("IncreaseSpeedKeybindButton") },
            { "DecreaseSpeed", root.Q<Button>("DecreaseSpeedKeybindButton") },
            { "ResetSpeed",    root.Q<Button>("ResetSpeedKeybindButton") },
            { "ZenMode",       root.Q<Button>("ZenModeKeybindButton") },
            { "Camera1",       root.Q<Button>("Camera1KeybindButton") },
            { "Camera2",       root.Q<Button>("Camera2KeybindButton") },
            { "Camera3",       root.Q<Button>("Camera3KeybindButton") },
            { "ToggleKeybindPanel", root.Q<Button>("ToggleKeybindPanelButton") }
        };

        // Loop through the actionButtonMap and set up the buttons with their corresponding actions
        foreach (var pair in actionButtonMap)
        {
            if (pair.Value != null)
            {
                pair.Value.clicked += () => StartKeyRebind(pair.Key);
                pair.Value.text = getKeyMap[pair.Key]().ToString();
            }
            else
            {
                Debug.LogError($"[SettingsController] Missing Button for {pair.Key}");
            }
        }

        // Initialize the language dropdown
        InitLanguageDropdown(root);
    }

    /// <summary>
    /// Updates the settings controller each frame.
    /// Checks for key input if waiting for a key to be selected.
    /// </summary>
    private void Update()
    {
        if (!waitingForKey) return;

        foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(key))
            {
                waitingForKey = false;
                onKeySelected?.Invoke(key);
                break;
            }
        }
    }

    /// <summary>
    /// Starts the key rebinding process for a specific action.
    /// </summary>
    private void StartKeyRebind(string action)
    {
        if (!actionButtonMap.TryGetValue(action, out var button) || button == null)
            return;

        waitingForKey = true;

        // Reset the button text to indicate waiting for input
        button.text = "|";

        // If a blink coroutine is already running, stop it
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
        }
        blinkCoroutine = StartCoroutine(BlinkText(button));

        onKeySelected = (key) =>
        {
            waitingForKey = false;

            // Stop the blinking coroutine when a key is selected
            if (blinkCoroutine != null)
            {
                StopCoroutine(blinkCoroutine);
                blinkCoroutine = null;
            }

            // Reset the element text color when done
            button.AddToClassList("visible-text");
            button.RemoveFromClassList("hidden-text");

            if (IsKeyAlreadyBound(key, action))
            {
                Debug.LogWarning($"[SettingsController] Key '{key}' already bound to another action.");
                button.text = getKeyMap[action]().ToString();
                return;
            }

            setKeyMap[action](key);
            button.text = key.ToString();

        };
    }

    /// <summary>
    /// Coroutine to blink the text of the button while waiting for a key input.
    /// The text will alternate between visible and hidden states.
    /// The text color is controlled by CSS classes "visible-text" and "hidden-text".
    /// </summary>
    /// <param name="element">The VisualElement to blink.</param>
    private IEnumerator BlinkText(VisualElement element)
    {
        bool visible = true;

        while (waitingForKey)
        {
            visible = !visible;

            if (visible)
            {
                element.AddToClassList("visible-text");
                element.RemoveFromClassList("hidden-text");
            }
            else
            {
                element.AddToClassList("hidden-text");
                element.RemoveFromClassList("visible-text");
            }

            // Blink interval
            yield return new WaitForSeconds(0.4f);
        }
    }

    /// <summary>
    /// Checks if the key is already bound to another action.
    /// </summary>
    private bool IsKeyAlreadyBound(KeyCode key, string currentAction)
    {
        foreach (var pair in getKeyMap)
        {
            if (pair.Key != currentAction && pair.Value() == key)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Initializes the language dropdown and its event handlers.
    /// </summary>
    private void InitLanguageDropdown(VisualElement root)
    {
        languageDropdown = root.Q<DropdownField>("LanguageDropdown");
        if (languageDropdown == null)
        {
            Debug.LogError("[SettingsController] LanguageDropdown not found!");
            return;
        }

        var locales = LocalizationSettings.AvailableLocales.Locales;
        var localeNames = new List<string>();

        foreach (var locale in locales)
        {
            localeNames.Add(locale.LocaleName);
        }

        languageDropdown.choices = localeNames;

        var currentLocale = LocalizationSettings.SelectedLocale;
        languageDropdown.value = currentLocale != null ? currentLocale.LocaleName : localeNames[0];

        languageDropdown.RegisterValueChangedCallback(evt =>
        {
            var selectedLocale = locales.Find(l => l.LocaleName == evt.newValue);
            if (selectedLocale != null && LocalizationManager.Instance != null)
            {
                LocalizationManager.Instance.SetLanguage(selectedLocale.Identifier.Code);
            }
        });
    }
}
