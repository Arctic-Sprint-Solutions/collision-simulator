// Description: Handles the About Scene UI logic

using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Recorder.OutputPath;

/// <summary>
/// Controller for the About UI
/// </summary>
public class AboutController : MonoBehaviour
{
    private UIDocument AboutUIDocument;
    private VisualElement _rootElement;
    private VisualElement aboutContainer;


    /// <summary>
    /// Called when the script instance is being loaded.
    /// </summary>
    private void OnEnable()
    {
        // Get the root visual element of the UI document
        _rootElement = AboutUIDocument.rootVisualElement;

        if (_rootElement == null)
        {
            Debug.LogError("Root element is null. Ensure the UIDocument is properly set up.");
            return;
        }

        var root = AboutUIDocument.rootVisualElement;
        aboutContainer = root.Q<VisualElement>("AboutContainer");


        if (aboutContainer != null)
        {
            aboutContainer.style.display = DisplayStyle.Flex; 
        }
    }

}
