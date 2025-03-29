using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;



    //  UI Manager logic for dropdown from Camera Managaer
    public class CameraDropdownUI : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    private DropdownField cameraDropdown;

    public delegate void CameraSelected(int index);
    public static event CameraSelected OnCameraSelected;


    // When used - fill dropdown with choices
    private void OnEnable()
    {
        CameraManagerNew.OnCamerasUpdated += PopulateDropdown;
    }


    // When not used - clear dropdown
    private void OnDisable()
    {
        CameraManagerNew.OnCamerasUpdated -= PopulateDropdown;
    }

    private void Start()
    {
        var root = uiDocument.rootVisualElement;
        cameraDropdown = root.Q<DropdownField>("CameraDropdown");

        if (cameraDropdown == null)
        {
            Debug.LogError("CameraDropdown UI element not found.");
            return;
        }

        cameraDropdown.RegisterValueChangedCallback(evt =>
        {
            int index = cameraDropdown.choices.IndexOf(evt.newValue);
            OnCameraSelected?.Invoke(index);
        });

        cameraDropdown.style.display = DisplayStyle.None; 
    }



    private void PopulateDropdown(List<string> cameraNames)
    {
        if (cameraDropdown == null) return;

        cameraDropdown.choices = cameraNames;
        cameraDropdown.value = cameraNames[0]; 
        cameraDropdown.label = "Select Camera";
        cameraDropdown.style.display = DisplayStyle.Flex;
    }
}