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

    [SerializeField] private UIDocument _cameraManagerDocument;
    private VisualElement _rootCam;
    private DropdownField _cameraDropdown;
    private VisualElement _cameraDropdownUI;

    private List<string> cameraKeys = new();

    public delegate void CameraSelected(int index);
    public static event CameraSelected OnCameraSelected;

    /// <summary>
    /// Flag to check if the initial value is set and prevent disabling the playable director
    /// </summary>
    private bool _isInititalValue = true;


    private void Awake()
    {
        if (_cameraManagerDocument == null)
        {
            Debug.LogError("CameraController: UIDocument is not assigned.");
            return;
        }

        InitializeCameraDropDownUI();
    }

    //When enabled - populates dropdown �I
    private void OnEnable()
    {
        Debug.Log("CameraController: OnEnable called. Initializing Camera Dropdown UI.");
        CameraManager.OnCamerasUpdated += PopulateDropdown;

        if (_cameraDropdown != null)
        {
            Debug.Log("CameraController: Registering value changed callback for Camera Dropdown.");
            _cameraDropdown.RegisterValueChangedCallback(evt => OnCameraChanged(evt.newValue));
        }
    }

    //When enabled - removes elements from dropdown �I
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
        if (_rootCam == null)
        {
            Debug.LogError("CameraController: Root VisualElement is null.");
            return;
        }


        // Get the camera dropdown and make it visible
        _cameraDropdownUI = _rootCam.Q<VisualElement>("CameraDropdownContainer");

        if (_cameraDropdownUI == null)
        {
            Debug.LogError("CameraController: cameraDropdownUI not found in the UI.");
            return;
        }

        _cameraDropdown = _cameraDropdownUI.Q<DropdownField>("CameraDropdown");
        if (_cameraDropdown == null)
        {
            Debug.LogError("CameraController: Camera DropdownField is null.");
            return;
        }
        
        Debug.Log("CameraController: Camera Dropdown UI initialized successfully.");

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

        _isInititalValue = false;
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
        if (selectedIndex < 0) return;

        // OnCameraSelected?.Invoke(selectedIndex);
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
        if (_cameraDropdownUI != null)
        {
            // _cameraDropdown.style.display = DisplayStyle.None;
            _cameraDropdownUI.AddToClassList("d-none");
        }
    }

    /// <summary>
    /// A function for showing the camera dropdown
    /// </summary>
    public void ShowDropdown()
    {
        if (_cameraDropdownUI != null)
        {
            // _cameraDropdown.style.display = DisplayStyle.Flex;
            _cameraDropdownUI.RemoveFromClassList("d-none");
        }
    }
}
