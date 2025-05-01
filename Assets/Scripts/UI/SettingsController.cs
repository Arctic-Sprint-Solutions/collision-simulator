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
    public UIDocument uiDocument;

    private Dictionary<string, Button> actionButtonMap;
    private Dictionary<string, Func<KeyCode>> getKeyMap;
    private Dictionary<string, Action<KeyCode>> setKeyMap;

    private DropdownField languageDropdown;

    private bool waitingForKey = false;
    private Action<KeyCode> onKeySelected;
    private Coroutine blinkCoroutine;

    private void Start()
    {
        StartCoroutine(SetupAsync());
    }

    private IEnumerator SetupAsync()
    {
        yield return new WaitUntil(() => InputManager.Instance != null && InputManager.Instance.keybinds != null);
        yield return LocalizationSettings.InitializationOperation;
        if (LocalizationManager.Instance != null)
            yield return new WaitUntil(() => LocalizationManager.Instance.IsLocalizationReady);

        InitializeMappings();
        InitializeUI();
    }

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
            { "ResetSpeed",    () => InputManager.Instance.keybinds.resetSpeedKey }
        };

        setKeyMap = new Dictionary<string, Action<KeyCode>>()
        {
            { "Pause",         (key) => InputManager.Instance.keybinds.pauseKey = key },
            { "Restart",       (key) => InputManager.Instance.keybinds.restartKey = key },
            { "Record",        (key) => InputManager.Instance.keybinds.recordKey = key },
            { "Download",      (key) => InputManager.Instance.keybinds.saveKey = key },
            { "IncreaseSpeed", (key) => InputManager.Instance.keybinds.increaseSpeedKey = key },
            { "DecreaseSpeed", (key) => InputManager.Instance.keybinds.decreaseSpeedKey = key },
            { "ResetSpeed",    (key) => InputManager.Instance.keybinds.resetSpeedKey = key }
        };
    }

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
            { "ResetSpeed",    root.Q<Button>("ResetSpeedKeybindButton") }
        };

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

        InitLanguageDropdown(root);
    }

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

    private void StartKeyRebind(string action)
    {
        if (!actionButtonMap.TryGetValue(action, out var button) || button == null)
            return;

        waitingForKey = true;

        // Reset the button text to indicate waiting for input
        button.text = "|";

        // If a blink coroutine is already running, stop it
        if( blinkCoroutine != null)
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
    /// <param name="element"></param>
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


    private bool IsKeyAlreadyBound(KeyCode key, string currentAction)
    {
        foreach (var pair in getKeyMap)
        {
            if (pair.Key != currentAction && pair.Value() == key)
                return true;
        }
        return false;
    }

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
