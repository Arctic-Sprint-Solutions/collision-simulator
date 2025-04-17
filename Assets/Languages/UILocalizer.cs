using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Components;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Localization.Tables;
using System.Collections;

///<summary>
///
///<summary>
public class UILocalizer : MonoBehaviour
{
    public UIDocument uiDocument;
    public string tableName = "UIStrings";

    private void Start()
    {
        StartCoroutine(ApplyLocalization());
    }

    ///<summary>
    ///
    ///<summary>
    private IEnumerator ApplyLocalization()
    {
        if (uiDocument == null)
        {
            Debug.LogError("UIDocument is not assigned!");
            yield break;
        }

        // Wait for Localization system to finish initializing
        yield return LocalizationSettings.InitializationOperation;

        // Load the string table
        var tableOp = LocalizationSettings.StringDatabase.GetTableAsync(tableName);
        yield return tableOp;

        if (tableOp.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("Failed to load localization table: " + tableName);
            yield break;
        }

        StringTable table = tableOp.Result;
        var root = uiDocument.rootVisualElement;

        // Traverse all Labels and update based on their name as key
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
}
