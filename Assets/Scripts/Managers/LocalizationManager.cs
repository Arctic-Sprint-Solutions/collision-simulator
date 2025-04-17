using UnityEngine;
using UnityEngine.Localization.Settings;
using System.Collections;

/// <summary>
/// Manages language settings and applies saved user preference.
/// </summary>
public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance { get; private set; }

    public bool IsLocalizationReady { get; private set; } = false;

    private void Awake()
    {
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

    ///<summary>
    ///Wait for localization system to finish initializing.
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

        IsLocalizationReady = true;
    }

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
