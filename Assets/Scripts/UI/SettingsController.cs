using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using System.Collections.Generic;

/// <summary>
/// Controller for the settings page, handling key rebinding and language selection.
/// </summary>
public class SettingsController : MonoBehaviour
{
    public UIDocument uiDocument;
    private Button pauseButton;
    private Button restartButton;
    private DropdownField languageDropdown;

    private bool waitingForKey = false;
    private Action<KeyCode> onKeySelected;

    private void Start()
    {
        // Start two coroutines: one for UI (keybinds) and one for ensuring localization is ready.
        StartCoroutine(WaitForInputManager());
        StartCoroutine(WaitForLocalization());
    }

    /// <summary>
    /// Wait until the InputManager and its keybinds are initialized, then set up UI elements.
    /// </summary>
    private IEnumerator WaitForInputManager()
    {
        while (InputManager.Instance == null || InputManager.Instance.keybinds == null)
        {
            yield return null;
        }

        var root = uiDocument.rootVisualElement;
        InitLanguageDropdown(root);

        pauseButton = root.Q<Button>("PauseButton");
        restartButton = root.Q<Button>("RestartButton");

        if (pauseButton != null)
        {
            pauseButton.clicked += () => StartKeyRebind("Pause");
            pauseButton.text = InputManager.Instance.keybinds.pauseKey.ToString();
        }
        else
        {
            Debug.LogError("PauseButton not found");
        }

        if (restartButton != null)
        {
            restartButton.clicked += () => StartKeyRebind("Restart");
            restartButton.text = InputManager.Instance.keybinds.restartKey.ToString();
        }
        else
        {
            Debug.LogError("RestartButton not found");
        }
    }

    /// <summary>
    /// Wait until the localization system and LocalizationManager are ready.
    /// </summary>
    private IEnumerator WaitForLocalization()
    {
        yield return LocalizationSettings.InitializationOperation;
        if (LocalizationManager.Instance != null)
        {
            yield return new WaitUntil(() => LocalizationManager.Instance.IsLocalizationReady);
        }
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

    /// <summary>
    /// Starts the key rebinding procedure for a given action.
    /// </summary>
    private void StartKeyRebind(string action)
    {
        if (InputManager.Instance == null || InputManager.Instance.keybinds == null)
        {
            Debug.LogError("InputManager or keybinds not initialized!");
            return;
        }

        waitingForKey = true;

        if (action == "Pause" && pauseButton != null)
        {
            pauseButton.text = "Press any key";
            onKeySelected = (key) =>
            {
                InputManager.Instance.keybinds.pauseKey = key;
                pauseButton.text = key.ToString();
            };
        }
        else if (action == "Restart" && restartButton != null)
        {
            restartButton.text = "Press any key";
            onKeySelected = (key) =>
            {
                InputManager.Instance.keybinds.restartKey = key;
                restartButton.text = key.ToString();
            };
        }
        else
        {
            Debug.LogError($"Unknown action '{action}' or button was null.");
        }
    }

    /// <summary>
    /// Configure the language dropdown to display all available languages and handle selection.
    /// </summary>
    private void InitLanguageDropdown(VisualElement root)
    {
        languageDropdown = root.Q<DropdownField>("LanguageDropdown");
        if (languageDropdown == null)
        {
            Debug.LogError("LanguageDropdown not found!");
            return;
        }

        var locales = LocalizationSettings.AvailableLocales.Locales;
        List<string> localeNames = new List<string>();

        foreach (var locale in locales)
        {
            // Use the human-friendly LocaleName in the dropdown.
            localeNames.Add(locale.LocaleName);
        }

        languageDropdown.choices = localeNames;

        // Set the dropdown's current value based on the currently selected locale.
        var currentLocale = LocalizationSettings.SelectedLocale;
        languageDropdown.value = currentLocale != null ? currentLocale.LocaleName : localeNames[0];

        // When the user selects a new language, update via the LocalizationManager.
        languageDropdown.RegisterValueChangedCallback(evt =>
        {
            var selectedName = evt.newValue;
            var selectedLocale = locales.Find(l => l.LocaleName == selectedName);
            if (selectedLocale != null && LocalizationManager.Instance != null)
            {
                LocalizationManager.Instance.SetLanguage(selectedLocale.Identifier.Code);
            }
        });
    }
}
