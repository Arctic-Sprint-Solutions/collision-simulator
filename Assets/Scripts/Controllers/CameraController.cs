// Description: Handles the camera dropdown UI logic

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;


/// <summary>
/// CameraController is responsible for managing the camera selection UI.
/// It populates a dropdown with available cameras and handles camera selection.
/// It listens for camera updates from the CameraManager and updates the UI accordingly.
/// </summary>
public class CameraController : MonoBehaviour
{
    /// <summary>
    /// A list of camera keys to be used for populating the dropdown.
    /// </summary>
    private List<string> cameraKeys = new();
    /// <summary>
    /// The dropdown field for selecting cameras in the UI.
    /// </summary>
    private DropdownField _cameraDropdown;
    /// <summary>
    /// The container for the camera dropdown in the UI.
    /// </summary>
    private VisualElement _cameraDropdownContainer;
    /// <summary>
    /// A delegate for handling camera selection events.
    /// </summary>
    /// <param name="index">The index of the selected camera.</param>
    public delegate void CameraSelected(int index);

    /// <summary>
    /// Initializes the CameraController and sets up event listeners.
    /// </summary>
    private void Start()
    {
        if(UIManager.Instance != null) 
        {   
            InitializeUI();
            Debug.Log("[CameraControlle] UI initialized successfully.");
        }

        if(CameraManager.Instance != null)
        {
            // Register the event listener for camera updates
            CameraManager.OnCamerasUpdated += PopulateDropdown;
        }

        if(InputManager.Instance != null)
        {
            // Register the event listener for camera key presses
            InputManager.Instance.OnCameraKeyPressed += (index) => OnCameraChanged(index);
        }
        
    }

    /// <summary>
    /// Initializes the UI elements for the camera dropdown.
    /// </summary>
    private void InitializeUI()
    {
        _cameraDropdownContainer = UIManager.Instance.GetElement<VisualElement>("CameraDropdownContainer");
        _cameraDropdown = UIManager.Instance.GetElement<DropdownField>("CameraDropdown");

        if(_cameraDropdown != null)
        {
            // Register the callback for when the camera dropdown value changes
            _cameraDropdown.RegisterValueChangedCallback(evt => OnCameraChanged(evt.newValue));
        }
    }

    /// <summary>
    /// Cleans up the CameraController by unregistering event listeners.
    /// </summary>
    private void OnDisable()
    {
        // Unregister event listeners to prevent memory leaks
        if (CameraManager.Instance != null)
        {
            CameraManager.OnCamerasUpdated -= PopulateDropdown;
        }

        // Unregister the value changed callback for the camera dropdown
        if (_cameraDropdown != null)
        {
            _cameraDropdown.UnregisterValueChangedCallback(evt => OnCameraChanged(evt.newValue));
        }

    }

    /// <summary>
    /// Populates the dropdown based on priority
    /// <param name="cameraNames">A list of camera names to populate the dropdown with.</param>
    /// </summary>
    private void PopulateDropdown(List<string> cameraNames)
    {
        if (_cameraDropdown == null) return;

        cameraKeys = cameraNames;

        UpdateDropdownOptions();
        ShowDropdown();
    }

    /// <summary>
    /// Updates the dropdown choices based on the current localization.
    /// This method retrieves the localized names for each camera key and updates the dropdown options accordingly.
    /// </summary>
    private void UpdateDropdownOptions()
    {
        if (_cameraDropdown == null || cameraKeys == null) return;

        List<string> localizedCameraNames = new List<string>();
        // Iterate through the camera keys and get their localized names
        foreach (var name in cameraKeys)
        {
            var localizedName = LocalizedUIHelper.Get(name);
            localizedCameraNames.Add(localizedName);
        }

        // Update the dropdown choices with the localized names and set the default value
        _cameraDropdown.choices = localizedCameraNames;
        _cameraDropdown.value = localizedCameraNames.FirstOrDefault();
    }

    /// <summary>
    /// Reloads localization for the dropdown when the language changes.
    /// </summary>
    public void ReloadLocalization()
    {
        UpdateDropdownOptions();
    }

    /// <summary>
    /// Handles camera selection from the dropdown
    ///  <param name="selectedCameraName">The selected camera name in the camera dropdown UI.</param>
    /// </summary>
    private void OnCameraChanged(string selectedCameraName)
    {
        if (cameraKeys == null || _cameraDropdown == null) return;
        
        // Find the index of the selected camera name in the cameraKeys list
        int selectedIndex = _cameraDropdown.choices.IndexOf(selectedCameraName);
        if (selectedIndex < 0) return;

        // Set the active camera in the CameraManager using the selected index
        CameraManager.Instance.SetActiveCamera(selectedIndex);

        // Update the dropdown value to reflect the selected camera
        _cameraDropdown.value = selectedCameraName;
    }

    /// <summary>
    /// Overloaded method to handle camera selection by index instead of name
    /// <param name="selectedIndex">The selected camera index in the camera dropdown UI.</param>
    /// </summary>
    private void OnCameraChanged(int selectedIndex)
    {
        if (cameraKeys == null || _cameraDropdown == null) return;

        if(selectedIndex >= 0 && selectedIndex < cameraKeys.Count)
        {
            CameraManager.Instance.SetActiveCamera(selectedIndex);
            // Find the name of the selected camera using the index
            string selectedCameraName = LocalizedUIHelper.Get(cameraKeys[selectedIndex]);
            if (string.IsNullOrEmpty(selectedCameraName))
            {
                Debug.LogError($"CameraController: Camera name is null or empty for index {selectedIndex}");
                return;
            }
            // Update the dropdown value to reflect the selected camera;
            _cameraDropdown.value = selectedCameraName;
        }
        else
        {
            Debug.LogError($"CameraController: Invalid camera index: {selectedIndex}");
        }

    }

    /// <summary>
    /// A function for hiding the camera dropdown
    /// </summary>
    public void HideDropdown()
    {
        if (_cameraDropdownContainer != null)
        {
            // _cameraDropdown.style.display = DisplayStyle.None;
            _cameraDropdownContainer.AddToClassList("d-none");
        }
    }

    /// <summary>
    /// A function for showing the camera dropdown
    /// </summary>
    public void ShowDropdown()
    {
        if (_cameraDropdownContainer != null)
        {
            Debug.Log("CameraController: ShowDropdown called.");
            _cameraDropdownContainer.RemoveFromClassList("d-none");
        }
    }
}