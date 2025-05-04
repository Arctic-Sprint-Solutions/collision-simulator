// Description: Handles the camera dropdown UI logic

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;


/// <summary>
/// Singleton class for camera dropdown UI and ensures that it persists across scenes
/// </summary>
public class CameraController : MonoBehaviour
{
    private List<string> cameraKeys = new();
    public delegate void CameraSelected(int index);
    private DropdownField _cameraDropdown;
    private VisualElement _cameraDropdownContainer;

    private void Start()
    {
        if(UIManager.Instance != null) 
        {   
            InitializeUI();
            Debug.Log("[CameraControlle] UI initialized successfully.");
        }

        if(CameraManager.Instance != null)
        {
            // Register CameraManager events
            CameraManager.OnCamerasUpdated += PopulateDropdown;
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


    //When enabled - removes elements from dropdown �I
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
        Debug.Log("CameraController: PopulateDropdown called with camera names: " + string.Join(", ", cameraNames));
        if (_cameraDropdown == null) return;

        cameraKeys = cameraNames;

        UpdateDropdownOptions();
        ShowDropdown();
    }

    /// <summary>
    /// Updates the dropdown choices based on the current localization.
    /// </summary>
    private void UpdateDropdownOptions()
    {
        if (_cameraDropdown == null || cameraKeys == null) return;

        List<string> localizedCameraNames = new List<string>();
        foreach (var name in cameraKeys)
        {
            var localizedName = LocalizedUIHelper.Get(name);
            localizedCameraNames.Add(localizedName);
        }

        _cameraDropdown.choices = localizedCameraNames;
        _cameraDropdown.value = localizedCameraNames.FirstOrDefault();
        _cameraDropdown.label = LocalizedUIHelper.Get("SelectCameraLabel");
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

        Debug.Log($"CameraController: OnCameraChanged called with selectedCameraName: {selectedCameraName}");
        int selectedIndex = _cameraDropdown.choices.IndexOf(selectedCameraName);
        Debug.Log($"CameraController: Selected Index: {selectedIndex}");
        if (selectedIndex < 0) return;

        CameraManager.Instance.SetActiveCamera(selectedIndex);
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
