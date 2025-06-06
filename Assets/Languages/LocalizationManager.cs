using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using System;
using System.Collections;

/// <summary>
/// Singleton that manages the app's current language setting
/// and notifies all subscribers when the language changes.
/// </summary>
public class LocalizationManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the LocalizationManager.
    /// </summary>
    public static LocalizationManager Instance { get; private set; }

    /// <summary>
    /// Event triggered whenever the localization is updated (e.g. language switch).
    /// </summary>
    public static event Action LocalizationUpdated;

    /// <summary>
    /// Indicates whether the localization system is ready.
    /// This is set to true after the initial language is loaded.
    /// Cached values are built at this point.
    /// </summary>
    public bool IsLocalizationReady { get; private set; } = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            StartCoroutine(LoadSavedLanguage());
            LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
            LocalizedUIHelper.BuildCache();

        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }

    /// <summary>
    /// Loads the saved language from PlayerPrefs or defaults to English.
    /// </summary>
    private IEnumerator LoadSavedLanguage()
    {
        yield return LocalizationSettings.InitializationOperation;

        string savedLang = PlayerPrefs.GetString("Language", "en");
        var locale = LocalizationSettings.AvailableLocales.Locales.Find(l => l.Identifier.Code == savedLang);

        if (locale != null)
        {
            LocalizationSettings.SelectedLocale = locale;
        }

        IsLocalizationReady = true;
        LocalizationUpdated?.Invoke();
    }

    /// <summary>
    /// Changes the app's language and saves the preference.
    /// </summary>
    /// <param name="localeCode">Locale code to switch to (e.g., "en", "no").</param>
    public void SetLanguage(string localeCode)
    {
        var locale = LocalizationSettings.AvailableLocales.Locales.Find(l => l.Identifier.Code == localeCode);

        if (locale != null)
        {
            LocalizationSettings.SelectedLocale = locale;
            PlayerPrefs.SetString("Language", localeCode);
            PlayerPrefs.Save();
            Debug.Log($"[LocalizationManager] Language changed to {localeCode}");

            LocalizationUpdated?.Invoke();
        }
    }

    /// <summary>
    /// Event handler for when the locale changes.
    /// </summary>
    private void OnLocaleChanged(Locale _)
    {
        LocalizationUpdated?.Invoke();
    }
}
