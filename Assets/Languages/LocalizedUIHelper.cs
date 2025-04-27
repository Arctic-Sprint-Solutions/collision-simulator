using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;

public static class LocalizedUIHelper
{
    private static StringTable _uiStringTable;
    private static bool _isReady = false;
    private static readonly string _defaultTable = "UIStrings";
    private static Dictionary<string, string> _textToKeyMap = new();

    /// <summary>
    /// Loads the UIStrings table once.
    /// </summary>
    public static async Task InitializeAsync(string tableName = null)
    {
        if (_isReady) return;
        // Wait for Unity's Localization system
        await LocalizationSettings.InitializationOperation.Task;

        // Fetch the table
        var handle = LocalizationSettings.StringDatabase.GetTableAsync(tableName ?? _defaultTable);
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            _uiStringTable = handle.Result;
            _isReady = true;
        // Build reverse lookup dictionary
            _textToKeyMap.Clear();
            foreach (var entry in _uiStringTable.Values)
            {
                if (entry == null) continue;
                string localized = entry.GetLocalizedString();
                if (!string.IsNullOrEmpty(localized))
                {
                    _textToKeyMap[localized] = entry.Key;
                }
            }
        }
        else
        {
            Debug.LogError($"LocalizedUIHelper: Failed to load table '{tableName ?? _defaultTable}'");
        }
    }

    public static string Get(string key)
    {
        if (!_isReady) return key;
        var entry = _uiStringTable.GetEntry(key);
        return entry != null ? entry.GetLocalizedString() : key;
    }

    /// <summary>
    /// Applies localization to a Label element by key.
    /// </summary>
    public static void Apply(Label label, string key)
    {
        if (label == null) return;
        label.text = Get(key);
    }

    /// <summary>
    /// Applies localization to a UIElements Button element by key.
    /// </summary>
    public static void Apply(Button button, string key)
    {
        if (button == null) return;
        button.text = Get(key);
    }
    /// <summary>
    /// Maps a localized string to its key.
    /// </summary>
    public static string GetKeyForText(string text)
    {
        if (string.IsNullOrEmpty(text)) return null;
        return _textToKeyMap.TryGetValue(text, out var key) ? key : null;
    }

}