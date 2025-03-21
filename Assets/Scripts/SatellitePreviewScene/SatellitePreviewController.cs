using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;

public class SatellitePreviewController : MonoBehaviour
{
    [SerializeField] private Transform satelliteContainer;
    [SerializeField] private UIDocument uiDocument;
    private GameObject instantiatedSatellite;
    private VisualElement _rootElement;

    private void OnEnable()
    {
        if(uiDocument == null)
        {
            Debug.LogError("UIDocument is not assigned in the inspector.");
            return;
        }

        // Get the root visual element of the UI document
        _rootElement = uiDocument.rootVisualElement;
        if (_rootElement == null)
        {
            Debug.LogError("Root element is null. Ensure the UIDocument is properly set up.");
            return;
        }

        SetupButtons();
    }

    private void SetupButtons()
    {
        // Finf the button contaier
        VisualElement buttonContainer = _rootElement.Q<VisualElement>("ButtonContainer");
        if(buttonContainer == null)
        {
            Debug.LogError("ButtonContainer not found in the UI.");
            return;
        }

        // Get the buttons
        Button button1 = buttonContainer.Q<Button>("Button1");
        if(button1 != null)
        {
            // Remove Unity classes
            button1.RemoveFromClassList("unity-button");
            button1.RemoveFromClassList("unity-text-element");
            Debug.Log("Button1 found and classes removed.");
        }

        Button button2 = buttonContainer.Q<Button>("Button2");
        if(button2 != null)
        {
            // Remove Unity classes
            button2.RemoveFromClassList("unity-button");
            button2.RemoveFromClassList("unity-text-element");
            Debug.Log("Button2 found and classes removed.");
        }
    }


    private void Start()
    {
        LoadSelectedSatellite();   
    }

    private void LoadSelectedSatellite()
    {
        // Get the selected satellite from the SimulationManager
        Satellite selectedSatellite = SimulationManager.Instance.SelectedSatellite;
        
        if (selectedSatellite == null)
        {
            Debug.LogError("Selected satellite is null.");
            return;
        }

        if(selectedSatellite.satellitePrefab == null)
        {
            Debug.LogError("Satellite prefab is null.");
            return;
        }

        // Instantiate the satellite prefab into the satellite container
        instantiatedSatellite = Instantiate(selectedSatellite.satellitePrefab, satelliteContainer);
    }

    private void OnDestroy()
    {
        if (instantiatedSatellite != null)
        {
            Destroy(instantiatedSatellite);
        }
    }
}
