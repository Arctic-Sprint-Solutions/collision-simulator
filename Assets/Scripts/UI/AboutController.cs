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
    private Label textBlock1;
    private Label textBlock2;
    private Label textBlock3;


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

        textBlock1 = root.Q<Label>("textBlock1");
        textBlock2 = root.Q<Label>("textBlock2");
        textBlock3 = root.Q<Label>("textBlock3");


        if (aboutContainer != null)
        {
            aboutContainer.style.display = DisplayStyle.Flex;
        }

        if (textBlock1 != null)
        {
            textBlock1.style.display = DisplayStyle.Flex;
            Debug.LogError("textBlock1 is null");
            textBlock1.text =@"New technology and the commercialization of satellite technology has 
caused more and more satellites to be sent into space.
When objects break apart or collide in space they create what is known 
as space debris. More research is needed to find the effects space debris
has on satellites, and spreading awareness is where it starts.

";
        }


        if (textBlock2 != null)
        {
            textBlock2.style.display = DisplayStyle.Flex;
            Debug.LogError("textBlock1 is null");
            textBlock2.text = @"More text here 

Ect

Ect

";
        }

        if (textBlock3 != null)
        {
            textBlock3.style.display = DisplayStyle.Flex;
            Debug.LogError("textBlock1 is null");
            textBlock3.text = @"And even more text here 

Ect

Ect

";
        }

    }

}
