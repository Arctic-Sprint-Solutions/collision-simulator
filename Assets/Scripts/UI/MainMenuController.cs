using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
  [SerializeField] private UIDocument menuUIDocument;

  private VisualElement _rootElement;

  private void OnEnable()
  {
      // Get the root visual element of the UI document
      _rootElement = menuUIDocument.rootVisualElement;

      if (_rootElement == null)
      {
          Debug.LogError("Root element is null. Ensure the UIDocument is properly set up.");
          return;
      }

      Debug.Log("Root element found: " + _rootElement.name);

    // Use schedule to ensure UI is fully loaded
    _rootElement.schedule.Execute(() => {
        InitializeNavLinks();
    });
  }

  private void InitializeNavLinks()
  {
      // Find the navigation links container
      VisualElement navLinksContainer = _rootElement.Q<VisualElement>("NavLinks");
      
      if (navLinksContainer == null)
      {
          Debug.LogError("Could not find NavLinks container in UI document");
          return;
      }
      
      // Find all elements with class "nav-link" under the container
      UQueryBuilder<VisualElement> linkQuery = navLinksContainer.Query(className: "nav-link");
      List<VisualElement> navLinkElements = linkQuery.ToList();
      
      Debug.Log($"Found {navLinkElements.Count} navigation links");
      
      foreach (VisualElement navLink in navLinkElements)
      {
        // Add click event listener to each link
        navLink.RegisterCallback<ClickEvent>(ev => OnNavLinkClicked(navLink));
      }
  }

    private void OnNavLinkClicked(VisualElement navLink)
    {
        // Handle the click event for the navigation link
        Debug.Log($"Navigation link clicked: {navLink.name}");
    }

}