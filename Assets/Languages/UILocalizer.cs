// Description: This script manages the localization of UI elements in Unity using the Localization package.
// It automatically updates the text of Label and Button elements based on the current language setting.

using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// Automatically localizes all Label elements inside a UIDocument.
/// Updates automatically when the app's language changes via the LocalizationManager.
/// </summary>
public class UILocalizer : MonoBehaviour
{
    /// <summary>
    /// The UIDocument containing UI elements to localize.
    /// </summary>
    public UIDocument uiDocument;
    
    /// <summary>
    /// Name of the localization table to pull text from. Default is "UIStrings".
    /// </summary>
    public string tableName = "UIStrings";

    private static StringTable cachedTable;
    private static Dictionary<string, string> localizedTextCache = new();

    /// <summary>
    /// Subscribes to localization events and verifies UIDocument.
    /// </summary>
    private void Awake()
    {
        if (uiDocument == null)
        {
            uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
                Debug.LogError("[UILocalizer] No UIDocument assigned or found!");
        }

        LocalizationManager.LocalizationUpdated += ReloadLocalization;
    }

    /// <summary>
    /// Unsubscribes from localization events to prevent memory leaks.
    /// </summary>
    private void OnDestroy()
    {
        LocalizationManager.LocalizationUpdated -= ReloadLocalization;
    }

    /// <summary>
    /// Begins initial localization on scene load.
    /// </summary>
    private void Start()
    {
        StartCoroutine(ApplyLocalizationCoroutine());
    }

    /// <summary>
    /// Reloads the localization data and updates all UI labels.
    /// Called automatically when the language changes.
    /// </summary>
    public void ReloadLocalization()
    {
        localizedTextCache.Clear();
        StartCoroutine(ApplyLocalizationCoroutine());
    }

    /// <summary>
    /// Coroutine that loads the StringTable, builds the cache, and updates labels.
    /// </summary>
    private IEnumerator ApplyLocalizationCoroutine()
    {
        if (uiDocument == null)
        {
            Debug.LogError("[UILocalizer] No UIDocument assigned!");
            yield break;
        }

        // Wait for the localization system to be initialized
        yield return LocalizationSettings.InitializationOperation;
        if (LocalizationManager.Instance != null)
            yield return new WaitUntil(() => LocalizationManager.Instance.IsLocalizationReady);

        // Allow one frame for the UIDocument hierarchy to be fully ready
        yield return null;

        // Always refetch the string table after reloads
        var tableOp = LocalizationSettings.StringDatabase.GetTableAsync(tableName);
        yield return tableOp;

        if (tableOp.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"[UILocalizer] Failed to load table '{tableName}'");
            yield break;
        }

        cachedTable = tableOp.Result;
        BuildCache();

        ApplyCachedLocalization();
    }

    /// <summary>
    /// Builds a cache mapping keys to localized strings for fast lookup.
    /// </summary>
    private void BuildCache()
    {
        localizedTextCache.Clear();

        if (cachedTable == null)
        {
            Debug.LogWarning("[UILocalizer] BuildCache called but cachedTable is null!");
            return;
        }

        foreach (var entry in cachedTable.Values)
        {
            if (entry != null && !string.IsNullOrEmpty(entry.Key))
            {
                localizedTextCache[entry.Key] = entry.GetLocalizedString();
            }
        }
    }

    /// <summary>
    /// Applies cached localization data to all Label elements under the UIDocument.
    /// </summary>
    private void ApplyCachedLocalization()
    {
        var root = uiDocument.rootVisualElement;
        if (root == null)
        {
            Debug.LogError("[UILocalizer] rootVisualElement is NULL");
            return;
        }

        // Localize all Labels
        foreach (var label in root.Query<Label>().ToList())
        {
            if (string.IsNullOrEmpty(label.name))
                continue;

            if (localizedTextCache.TryGetValue(label.name, out var localizedText))
            {
                LocalizedUIHelper.Apply(label, label.name);
            }
        }

        // Localize all Buttons
        foreach (var button in root.Query<Button>().ToList())
        {
            if (string.IsNullOrEmpty(button.name))
                continue;

            if (localizedTextCache.TryGetValue(button.name, out var localizedText))
            {
                LocalizedUIHelper.Apply(button, button.name);
            }
        }
    }
}
