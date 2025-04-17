using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using System.Collections.Generic;

///<summary>
///Controller for the settingspage.
///<summary>
public class SettingsController : MonoBehaviour
{
    public UIDocument uiDocument;
    private Button pauseButton;
    private Button restartButton;
    private DropdownField languageDropdown;

    private bool waitingForKey = false;
    private Action<KeyCode> onKeySelected;

    ///<summary>
    ///Offset start timing to ensure UI is ready.
    ///<summary>
    private void Start()
    {
        StartCoroutine(InitUI());
        StartCoroutine(LoadSavedLanguage());
    }

    ///<summary>
    ///Delay until InputManager is ready, fetches root UI, and handles the rebind button presses.
    ///<summary>
    private IEnumerator InitUI()
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

    ///<summary>
    ///
    ///<summary>
    private IEnumerator LoadSavedLanguage()
    {
        yield return LocalizationSettings.InitializationOperation;

        string savedLang = PlayerPrefs.GetString("Language", "en");
        var locale = LocalizationSettings.AvailableLocales.Locales
            .Find(l => l.Identifier.Code == savedLang);

        if (locale != null)
        {
            LocalizationSettings.SelectedLocale = locale;
        }
    }

    ///<summary>
    ///Registers the pressed key.
    ///<summary>
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

    ///<summary>
    ///Handles new binds according to the button pressed.
    ///<summary>
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

    ///<summary>
    ///Handles the binding of Localization and the settings UI
    ///<summary>
    private void InitLanguageDropdown(VisualElement root)
    {
        languageDropdown = root.Q<DropdownField>("LanguageDropdown");

        var locales = LocalizationSettings.AvailableLocales.Locales;
        List<string> localeNames = new List<string>();

        foreach (var locale in locales)
        {
            localeNames.Add(locale.LocaleName); // This is the readable name shown in the dropdown
        }

        languageDropdown.choices = localeNames;

        // Set current selection
        var currentLocale = LocalizationSettings.SelectedLocale;
        languageDropdown.value = currentLocale?.LocaleName ?? localeNames[0];

        // Handle selection changes
        languageDropdown.RegisterValueChangedCallback(evt =>
        {
            var selectedName = evt.newValue;
            var selectedLocale = locales.Find(l => l.LocaleName == selectedName);

            if (selectedLocale != null)
            {
                LocalizationManager.Instance.SetLanguage(selectedLocale.Identifier.Code);
            }
        });
    }
}
