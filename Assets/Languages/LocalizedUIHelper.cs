// Description: This script acts as a helper for UI localization and caching translated strings for performance.

using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.UIElements;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// Provides helpers for UI localization, including caching translated strings.
/// </summary>
public static class LocalizedUIHelper
{
    private static StringTable uiStringTable;
    private static bool isReady = false;
    private static readonly Dictionary<string, string> cache = new();
    private static readonly string defaultTable = "UIStrings";

    /// <summary>
    /// Initializes and loads the string table from localization system.
    /// </summary>
    public static async Task InitializeAsync(string tableName = null)
    {
        if (isReady) return;

        await LocalizationSettings.InitializationOperation.Task;

        var handle = LocalizationSettings.StringDatabase.GetTableAsync(tableName ?? defaultTable);
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            uiStringTable = handle.Result;
            isReady = true;
            BuildCache();
        }
        else
        {
            Debug.LogError($"[LocalizedUIHelper] Failed to load table: {tableName ?? defaultTable}");
        }
    }

    /// <summary>
    /// Forces a re-fetch of the current string table and rebuilds the cache.
    /// Should be called after language switching.
    /// </summary>
    public static void ReloadTable()
    {
        isReady = false;
        _ = InitializeAsync();
    }

    /// <summary>
    /// Refreshes the in-memory cache from the currently loaded table.
    /// </summary>
    public static void BuildCache()
    {
        if (uiStringTable == null)
        {
            return;
        }

        cache.Clear();
        foreach (var entry in uiStringTable.Values)
        {
            if (entry != null && !string.IsNullOrEmpty(entry.Key))
            {
                cache[entry.Key] = entry.GetLocalizedString();
            }
        }
    }

    public static string Get(string key)
    {
        if (!isReady || string.IsNullOrEmpty(key)) return key;
        if (cache.TryGetValue(key, out var localized))
            return localized;

        var entry = uiStringTable?.GetEntry(key);
        if (entry != null)
        {
            var value = entry.GetLocalizedString();
            cache[key] = value;
            return value;
        }

        Debug.LogWarning($"[LocalizedUIHelper] Missing key: {key}");
        return key;
    }

    public static void Apply(Label label, string key)
    {
        if (label == null) return;
        label.text = Get(key);
    }

    public static void Apply(Button button, string key)
    {
        if (button == null) return;
        button.text = Get(key);
    }
}
