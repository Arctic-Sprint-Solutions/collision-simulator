// Description: Handles the camera dropdown UI logic

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// Camera Controller for camera UI elements
public class CameraController : MonoBehaviour
{
    [SerializeField] private UIDocument _cameraManagerDocument;
    private VisualElement _rootCam;
    private DropdownField _cameraDropdown;
    private VisualElement _cameraDropdownUI;

    public delegate void CameraSelected(int index);
    public static event CameraSelected OnCameraSelected;

    //Instansiated as Singleton
    private void Awake()
    {
        InitializeCameraDropDownUI();
    }

    //When enabled - populates dropdown ÙI
    private void OnEnable()
    {
        CameraManager.OnCamerasUpdated += PopulateDropdown;

        if (_cameraDropdown != null)
        {
            _cameraDropdown.RegisterValueChangedCallback(evt => OnCameraChanged(evt.newValue));
        }
    }

    //When enabled - removes elements from dropdown ÙI
    private void OnDisable()
    {
        CameraManager.OnCamerasUpdated -= PopulateDropdown;
    }

    /// <summary>
    /// Initializes the UI elements for camera selection
    /// </summary>
    private void InitializeCameraDropDownUI()
    {
        _rootCam = _cameraManagerDocument.rootVisualElement;

        // Get the camera dropdown and make it visible
        _cameraDropdownUI = _rootCam.Q<VisualElement>("CameraDropdown");
        _cameraDropdownUI.RemoveFromClassList("d-none");

        _cameraDropdown = _cameraDropdownUI.Q<DropdownField>("CameraDropdown");
    }

    /// <summary>
    /// Populates the dropdown with available camera names
    /// </summary>
    private void PopulateDropdown(List<string> cameraNames)
    {
        if (_cameraDropdown == null) return;

        _cameraDropdown.choices = cameraNames;
        _cameraDropdown.value = cameraNames[0]; // Default to the first camera
        _cameraDropdown.label = "Select Camera";
        _cameraDropdown.style.display = DisplayStyle.Flex;
    }

    /// <summary>
    /// Handles camera selection from the dropdown
    /// </summary>
    private void OnCameraChanged(string selectedCameraName)
    {
        int selectedIndex = _cameraDropdown.choices.IndexOf(selectedCameraName);
        OnCameraSelected?.Invoke(selectedIndex);
    }

    /// <summary>
    /// A function for hiding the camera dropdown
    /// </summary>
    public void HideDropdown()
    {
        if (_cameraDropdown != null)
        {
            _cameraDropdown.style.display = DisplayStyle.None;
        }
    }

    /// <summary>
    /// A function for showing the camera dropdown
    /// </summary>
    public void ShowDropdown()
    {
        if (_cameraDropdown != null)
        {
            _cameraDropdown.style.display = DisplayStyle.Flex;
        }
    }
}
