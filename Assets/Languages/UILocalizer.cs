using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Localization.Tables;
using System.Collections;
using System.Linq;

/// <summary>
/// Applies localized text to UI elements using the assigned UIDocument.
/// </summary>
public class UILocalizer : MonoBehaviour
{
    public UIDocument uiDocument;
    public string tableName = "UIStrings";

    private void Start()
    {
        StartCoroutine(ApplyLocalization());
    }

    private IEnumerator ApplyLocalization()
    {
        if (uiDocument == null)
        {
            Debug.LogError("UIDocument is not assigned!");
            yield break;
        }

        // Ensure the localization system and our saved language are ready.
        yield return LocalizationSettings.InitializationOperation;
        if (LocalizationManager.Instance != null)
        {
            yield return new WaitUntil(() => LocalizationManager.Instance.IsLocalizationReady);
        }

        // Load the string table for the current locale.
        var tableOp = LocalizationSettings.StringDatabase.GetTableAsync(tableName);
        yield return tableOp;

        if (tableOp.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("Failed to load localization table: " + tableName);
            yield break;
        }

        StringTable table = tableOp.Result;
        var root = uiDocument.rootVisualElement;

        // Traverse all Label elements using their 'name' field as the key.
        foreach (var element in root.Query<Label>().ToList())
        {
            if (string.IsNullOrEmpty(element.name))
                continue;

            var entry = table.GetEntry(element.name);
            if (entry != null)
            {
                element.text = entry.GetLocalizedString();
            }
            else
            {
                Debug.LogWarning($"No localization entry found for key: {element.name}");
            }
        }
    }

    private void OnEnable()
    {
        // Subscribe to changes so that any language updates refresh the UI.
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
    }

    private void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }

    private void OnLocaleChanged(UnityEngine.Localization.Locale locale)
    {
        // Reapply localization when the locale is changed.
        StartCoroutine(ApplyLocalization());
    }
}
