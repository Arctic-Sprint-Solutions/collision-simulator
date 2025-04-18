// Description: Handles the About Scene UI logic

using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Controller for the About UI
/// </summary>
public class AboutController : MonoBehaviour
{
    private UIDocument AboutUIDocument;

    private VisualElement aboutContainer;
    private Label introText;
    /// <summary>
    /// Called when the script instance is being loaded.
    /// </summary>
    private void OnEnable()
    {
        // Get the UI document
        AboutUIDocument = GetComponent<UIDocument>();

        if (AboutUIDocument == null)
        {
            Debug.LogError("AboutUIDocument not found.");
            return;
        }

        //Get the UI root
        var root = AboutUIDocument.rootVisualElement;

        if (root == null)
        {
            Debug.LogError("Root VisualElement not found.");
            return;
        }

        aboutContainer = root.Q<VisualElement>("AboutContainer");

        introText = root.Q<Label>("introText");


        if (aboutContainer != null)
        {
            aboutContainer.style.display = DisplayStyle.Flex;
        }

        if (introText != null)
        {
            introText.style.display = DisplayStyle.Flex;
            Debug.LogError("introText is null");
            introText.text =@"New technology and the commercialization of satellite technology has 
caused more and more satellites to be sent into space.
When objects break apart or collide in space they create what is known 
as space debris. More research is needed to find the effects space debris
has on satellites, and spreading awareness is where it starts.

";
        }
    }

}
