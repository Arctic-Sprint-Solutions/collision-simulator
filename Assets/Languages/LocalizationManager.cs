using UnityEngine;
using UnityEngine.Localization.Settings;
using System.Collections;

/// <summary>
/// Manages language settings and applies saved user preference.
/// This object is marked as DontDestroyOnLoad so it is available across scenes.
/// </summary>
public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance { get; private set; }
    public bool IsLocalizationReady { get; private set; } = false;

    private void Awake()
    {
        // Ensure this manager persists and is unique.
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            StartCoroutine(LoadSavedLanguage());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Wait for the localization system to finish initializing, then load the saved language.
    /// </summary>
    private IEnumerator LoadSavedLanguage()
    {
        // Wait until Unity's Localization system is fully initialized.
        yield return LocalizationSettings.InitializationOperation;

        // Retrieve the saved language from PlayerPrefs (defaulting to English "en").
        string savedLang = PlayerPrefs.GetString("Language", "en");

        // Look for the corresponding locale and set it.
        var locale = LocalizationSettings.AvailableLocales.Locales
            .Find(l => l.Identifier.Code == savedLang);
        if (locale != null)
        {
            LocalizationSettings.SelectedLocale = locale;
        }

        IsLocalizationReady = true; // Notify that the language is loaded.
    }

    /// <summary>
    /// Sets a new language: updates the locale, saves the preference, and applies the change immediately.
    /// </summary>
    public void SetLanguage(string localeCode)
    {
        var locale = LocalizationSettings.AvailableLocales.Locales
            .Find(l => l.Identifier.Code == localeCode);
        if (locale != null)
        {
            LocalizationSettings.SelectedLocale = locale;
            PlayerPrefs.SetString("Language", localeCode);
            PlayerPrefs.Save();
        }
    }
}
