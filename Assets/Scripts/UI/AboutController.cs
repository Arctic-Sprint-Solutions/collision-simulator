// Description: Handles the About Scene UI and visual elements

using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Class for About Controller handling the About Scene visual elements
/// </summary>
public class AboutController : MonoBehaviour
{
    // Reference to the UI Document for the About Scene
    private UIDocument AboutUIDocument;

    // Reference to the container with the text blocks
    private VisualElement aboutContainer;

    // Reference to the text blocks in the scene
    private Label textBlock0;
    private Label textBlock1;
    private Label textBlock2;
    private Label textBlock3;
    private Label textBlock4;
    private Label textBlock5;


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

        //Get the UI root from the AboutUIDocument
        var root = AboutUIDocument.rootVisualElement;

        if (root == null)
        {
            Debug.LogError("Root VisualElement not found.");
            return;
        }

        // Creating a visual element for the container
        aboutContainer = root.Q<VisualElement>("AboutContainer");

        // Creating visual elements for the text blocks
        textBlock0 = root.Q<Label>("textBlock0");
        textBlock1 = root.Q<Label>("textBlock1");
        textBlock2 = root.Q<Label>("textBlock2");
        textBlock3 = root.Q<Label>("textBlock3");
        textBlock4 = root.Q<Label>("textBlock4");
        textBlock5 = root.Q<Label>("textBlock5");
        


        if (aboutContainer != null)
        {
            // Displays the aboutContainer
            aboutContainer.style.display = DisplayStyle.Flex;
        }

        
        if (textBlock0 != null)
        {
            // Displays textblock0
            textBlock0.style.display = DisplayStyle.Flex;
            textBlock0.text = @"New technology and the commercialization of satellite systems have led to an increasing number of satellites being launched into space. When objects break apart or collide in orbit, they create what is known as space debris. More research is needed to understand the effects space debris has on satellites, but raising awareness is where it begins.

";
        }

        if (textBlock1 != null)
        {
            // Displays textblock1
            textBlock1.style.display = DisplayStyle.Flex;
            textBlock1.text = @"Orbital Watch is a visual tool created to simulate simple collisions between satellites and space debris. Developed using the Unity game engine and Blender for 3D modeling, Orbital Watch provides a visual representation of collisions in space. Satellite models have been imported from NASA’s publicly available 3D model library.

• Show students the growing challenge of space debris 
• Observe collisions from different angles and how they affect various satellites  
• Learn how space debris is a growing concern and why further research is needed"";


";
        }


        if (textBlock2 != null)
        {
            // Displays textblock2
            textBlock2.style.display = DisplayStyle.Flex;
            textBlock2.text = @"During the spring of 2025 a project was added to the Bachelor of Computer Science at UiT The Arctic University of Norway called “Visualization of collisions between satellites and space debris”. The project’s main requirements were to create a visual tool which could simulate simple collisions between satellites and space debris. The tool was to be used during teaching so that more students (and other potential stakeholders) can learn more about the effects of space debris. 
 
The client requesting the project was Pål Gunnar Ellingsen, an Associate professor within satelite technology and remote sensing at the Department of Electrical Engineering at UiT. The head advisor the project was Frode Nesje, Assistant Professor at the Department of Computer Science and Computational Engineering at UiT.

The project was taken on by the dev team of Arctic Sprint Solutions 2.0, consisting of four graduating students in the field of Bachelor of Computer Science (Datateknikk) at UiT: Madeleine Woodbury, Ola Giæver, Gabriel Halstensen and Ingrid Ledingham. 



";
        }

        if (textBlock3 != null)
        {
            // Displays textblock3
            textBlock3.style.display = DisplayStyle.Flex;
            textBlock3.text = @"The main challenge of simulating collisons between space debris is the lack of research on how collisions make objects behave in space. Objects moving through orbit typically travel at extremely high speeds - as much as 7 kilometers per second (25 000 kilomters/hour). Comparatively a rifle bullet moves at the speed of 1.2 kilomters/second, a mere fraction of this. When two objects collide in orbit at such velocities, the realtive collision speed might reach as much as 15 kilomters/second (54 000 kilometers/hour). Simulating such high speed collisions can therefore be difficult. Both when simulating objects through physical experiments, as well as when visualizing collisions with tools such as Orbit Watch. It is hard to mimic collision effects that there is little to no real-world data available.


";
        }

        if (textBlock4 != null)
        {
            // Displays textblock4
            textBlock4.style.display = DisplayStyle.Flex;
            textBlock4.text = @"In order to create a visual tool for space debris collisions, it has been important to set aside attempts to capture how objects actually behave in space. The project has been commissionedto simulate simple collisions, not to provide accurate representations of how objects physically behave in orbit when colliding. 


";
        }

        if (textBlock5 != null)
        {
            // Displays textblock5
            textBlock5.style.display = DisplayStyle.Flex;
            textBlock5.text = @"Although Orbital Watch was created to simulate simple collisions, it holds potential for further development — possibly as part of a Master of Applied Science assignment. Future improvements could include more research into real space physics, enabling more accurate depictions of collisions in orbit. One might also consider experimenting with more advanced game engines, such as Unreal, to enhance the graphical quality and allow for greater customization.

Further development would likely require additional time and resources to reach a higher level of simulation fidelity. But much like in the world of video games, there’s always room for a sequel — one with higher resolution and even more content.


";
        }

    }

}
