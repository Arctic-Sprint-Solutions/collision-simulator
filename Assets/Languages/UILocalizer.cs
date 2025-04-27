using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Localization.Tables;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

/// <summary>
/// Applies localized text to UI elements using the assigned UIDocument.
/// </summary>
public class UILocalizer : MonoBehaviour
{
    [Tooltip("The UI Document containing elements to localize.")]
    public UIDocument uiDocument;

    [Tooltip("Name of the string table for UI localization.")]
    public string tableName = "UIStrings";

    private static StringTable _cachedTable;

    private void Awake()
    {
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
    }

    private void OnDestroy()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }

    private void OnLocaleChanged(UnityEngine.Localization.Locale locale)
    {
        StartCoroutine(ApplyLocalization());
    }

    private void Start()
    {
        StartCoroutine(ApplyLocalization());
    }

    private IEnumerator ApplyLocalization()
    {
        if (uiDocument == null)
        {
            Debug.LogError("UILocalizer: UIDocument is not assigned!");
            yield break;
        }

        // Wait for localization system initialization
        yield return LocalizationSettings.InitializationOperation;
        if (LocalizationManager.Instance != null)
            yield return new WaitUntil(() => LocalizationManager.Instance.IsLocalizationReady);

        // Load the string table
        var tableOp = LocalizationSettings.StringDatabase.GetTableAsync(tableName);
        yield return tableOp;

        if (tableOp.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"UILocalizer: Failed to load table '{tableName}'");
            yield break;
        }

        _cachedTable = tableOp.Result;

        // Get root and ensure it's valid
        var root = uiDocument.rootVisualElement;
        if (root == null)
        {
            Debug.LogWarning("UILocalizer: rootVisualElement is null, skipping localization.");
            yield break;
        }

        // Localize all labeled elements
        foreach (var label in root.Query<Label>().ToList())
        {
            if (string.IsNullOrEmpty(label.name))
                continue;

            var entry = _cachedTable.GetEntry(label.name);
            if (entry != null)
                label.text = entry.GetLocalizedString();
        }
    }
}
