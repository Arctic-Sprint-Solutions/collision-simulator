// Description: Handles the main menu UI logic

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Controller for the main menu UI
/// </summary>
public class MainMenuController : MonoBehaviour
{
  [SerializeField] private UIDocument menuUIDocument;
  [SerializeField] private AppSettings appSettings;
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

    // Set the title and subtitle of the main menu
    SetMainMenuTitle();

    // Use schedule to ensure UI is fully loaded
    _rootElement.schedule.Execute(() => {
        InitializeNavLinks();
    });
  }

  /// <summary>
  /// Sets the title and subtitle of the main menu
  /// </summary>
  private void SetMainMenuTitle()
  {
    if(appSettings == null)
    {
        Debug.LogError("AppSettings is not assigned.");
        return;
    }

    // Find the title element and set its text
    _rootElement.Q<Label>("MainTitle").text = appSettings.appName;
  
    // Find the subtitle element and set its text
    _rootElement.Q<Label>("Subtitle").text = appSettings.appSubtitle;
    
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
  /// Handles the click event for the navigation link
  /// </summary>
  private void OnNavLinkClicked(VisualElement navLink)
  {
    // Handle the click event for the navigation link
    Debug.Log($"Navigation link clicked: {navLink.name}");
    if(navLink.name == "ReturnToEarth") {
      SimulationManager.Instance.QuitApplication();
    } else {
      // TODO: Implement navigation logic
      // For now, just load the SatellitesGridScene
      SimulationManager.Instance.LoadScene("SatellitesGridScene");
    }
  }

}