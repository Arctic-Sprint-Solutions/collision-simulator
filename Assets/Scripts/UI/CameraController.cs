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

    /// <summary>
    /// Flag to check if the initial value is set and prevent disabling the playable director
    /// </summary>
    private bool _isInititalValue = true;


    //When enabled - populates dropdown �I
    private void OnEnable()
    {
        Debug.Log("CameraController: OnEnable called. Initializing Camera Dropdown UI.");
        CameraManager.OnCamerasUpdated += PopulateDropdown;

        if (UIManager.Instance.CameraDropdown != null)
        {
            Debug.Log("CameraController: Registering value changed callback for Camera Dropdown.");
            // Register the callback for when the camera dropdown value changes
            UIManager.Instance.CameraDropdown.RegisterValueChangedCallback(evt => OnCameraChanged(evt.newValue));
        }
    }

    //When enabled - removes elements from dropdown �I
    private void OnDisable()
    {
        CameraManager.OnCamerasUpdated -= PopulateDropdown;
    }

    /// <summary>
    /// Populates the dropdown based on priority
    /// <param name="cameraNames">A list of camera names to populate the dropdown with.</param>
    /// </summary>
    private void PopulateDropdown(List<string> cameraNames)
    {
        Debug.Log("CameraController: PopulateDropdown called with camera names: " + string.Join(", ", cameraNames));
        // if (_cameraDropdown == null) return;

        cameraKeys = cameraNames;

        UpdateDropdownOptions();
        ShowDropdown();

        _isInititalValue = false;
    }

    /// <summary>
    /// Updates the dropdown choices based on the current localization.
    /// </summary>
    private void UpdateDropdownOptions()
    {
        if (UIManager.Instance.CameraDropdown == null || cameraKeys == null) return;

        List<string> localizedCameraNames = new List<string>();
        foreach (var name in cameraKeys)
        {
            var localizedName = LocalizedUIHelper.Get(name);
            localizedCameraNames.Add(localizedName);
        }

        UIManager.Instance.CameraDropdown.choices = localizedCameraNames;
        UIManager.Instance.CameraDropdown.value = localizedCameraNames.FirstOrDefault();
        UIManager.Instance.CameraDropdown.label = LocalizedUIHelper.Get("SelectCameraLabel");
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
        if (cameraKeys == null || UIManager.Instance.CameraDropdown == null) return;

        Debug.Log($"CameraController: OnCameraChanged called with selectedCameraName: {selectedCameraName}");
        int selectedIndex = UIManager.Instance.CameraDropdown.choices.IndexOf(selectedCameraName);
        Debug.Log($"CameraController: Selected Index: {selectedIndex}");
        if (selectedIndex < 0) return;

        if(!_isInititalValue) 
        {
            CameraManager.Instance.DisablePlayableDirector();
        }
        CameraManager.Instance.SetActiveCamera(selectedIndex);
    }

    /// <summary>
    /// A function for hiding the camera dropdown
    /// </summary>
    public void HideDropdown()
    {
        if (UIManager.Instance.CameraDropdownUI != null)
        {
            // _cameraDropdown.style.display = DisplayStyle.None;
            UIManager.Instance.CameraDropdownUI.AddToClassList("d-none");
        }
    }

    /// <summary>
    /// A function for showing the camera dropdown
    /// </summary>
    public void ShowDropdown()
    {
        if (UIManager.Instance.CameraDropdownUI != null)
        {
            // _cameraDropdown.style.display = DisplayStyle.Flex;
            UIManager.Instance.CameraDropdownUI.RemoveFromClassList("d-none");
        }
    }
}
