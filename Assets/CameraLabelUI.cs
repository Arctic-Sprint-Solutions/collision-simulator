using UnityEngine;
using UnityEngine.UIElements;

// Description: Script assigning the style to the camera label.
public class CameraLabelUI : MonoBehaviour
{

    // Defines a UIDocument - a Component that connects VisualElements to GameObjects. 
    public UIDocument uiDocument; 

    private void Start()
    {

        VisualElement root = uiDocument.rootVisualElement;

        Label CameraLabel = root.Q<Label>("CameraLabel");

        // Apply the style 
        CameraLabel.AddToClassList("camera-label");
    }
}
