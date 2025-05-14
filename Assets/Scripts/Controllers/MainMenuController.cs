// Description: Handles the main menu UI logic

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Controller for the main menu UI
/// This class manages the navigation links and their click events.
/// </summary>
public class MainMenuController : MonoBehaviour
{
    /// <summary>
    /// Reference to the UIDocument component for accessing UI elements.
    /// </summary>
    [SerializeField] private UIDocument menuUIDocument;
    /// <summary>
    /// Reference to the root visual element of the UI document.
    /// </summary>
    private VisualElement _rootElement;

    /// <summary>
    /// Called when the script instance is being loaded.
    /// </summary>
    private void OnEnable()
    {
        // Get the root visual element of the UI document
        _rootElement = menuUIDocument.rootVisualElement;

        if (_rootElement == null)
        {
            Debug.LogError("Root element is null. Ensure the UIDocument is properly set up.");
            return;
        }

        // Use schedule to ensure UI is fully loaded
        _rootElement.schedule.Execute(() =>
        {
            InitializeNavLinks();
        });
    }


    /// <summary>
    /// Initializes the navigation links in the main menu and adds click event listeners
    /// </summary>
    private void InitializeNavLinks()
    {
        // Find the navigation links container
        VisualElement navLinksContainer = _rootElement.Q<VisualElement>("NavLinks");

        if (navLinksContainer == null)
        {
            Debug.LogError("Could not find NavLinks container in UI document");
            return;
        }

        // Find all elements with class "nav-link" in the container
        UQueryBuilder<VisualElement> linkQuery = navLinksContainer.Query(className: "nav-link");
        List<VisualElement> navLinkElements = linkQuery.ToList();


        foreach (VisualElement navLink in navLinkElements)
        {
            // Add click event listener to each link
            navLink.RegisterCallback<ClickEvent>(ev => OnNavLinkClicked(navLink));
        }
    }

    /// <summary>
    /// Handles the click event for navigation links to scenes
    /// </summary>
    private readonly Dictionary<string, string> sceneMap = new Dictionary<string, string>
    {
     	//Scene map dictionrary - maps nav links to scene names
    	{ "LoadSatellites", "SatellitesGridScene" },
        { "ViewSpaceDebris", "SpaceDebrisScene" },
        { "About", "AboutScene" },
        {"Settings", "SettingsScene"}
    };

    /// <summary>
    /// Called when a navigation link is clicked    
    /// Handles the click event for navigation links to scenes
    /// </summary>
    private void OnNavLinkClicked(VisualElement navLink)
    {
        Debug.Log($"Navigation link clicked: {navLink.name}");

        // Exit the application if the "ReturnToEarth" link is clicked
        if (navLink.name == "ReturnToEarth")
        {
            SimulationManager.Instance.QuitApplication();
            return;
        }

        // Check if the clicked link has a corresponding scene in the map
        if (sceneMap.TryGetValue(navLink.name, out string sceneName))
        {
            // Load the corresponding scene
            SimulationManager.Instance.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning($"No scene mapped for nav link: {navLink.name}");
        }
    }
}