// Description: Handles the camera dropdown UI logic

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;


/// <summary>
/// Initializes the singleton instance and ensures that it persists across scenes
/// </summary>
public class CameraController : MonoBehaviour
{

    [SerializeField] private UIDocument _cameraManagerDocument;
    private VisualElement _rootCam;
    private DropdownField _cameraDropdown;
    private VisualElement _cameraDropdownUI;

    public delegate void CameraSelected(int index);
    public static event CameraSelected OnCameraSelected;

    //Camera priorities enabling switch
    private int defaultPriority = 10;
    private int activePriority = 20;


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
        // _cameraDropdownUI.RemoveFromClassList("d-none");

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
    /// </summary>
    private void PopulateDropdown(List<string> cameraNames)
    {
        Debug.Log("CameraController: PopulateDropdown called with camera names: " + string.Join(", ", cameraNames));
        if (_cameraDropdown == null) return;

        _cameraDropdown.choices = cameraNames;
        _cameraDropdown.value = cameraNames[0]; 
        _cameraDropdown.label = "Select Camera";
        // _cameraDropdown.style.display = DisplayStyle.Flex;
        ShowDropdown();

    }



    /// <summary>
    /// Handles camera selection from the dropdown
    /// </summary>
    private void OnCameraChanged(string selectedCameraName)
    {
        Debug.Log($"CameraController: OnCameraChanged called with selectedCameraName: {selectedCameraName}");
        int selectedIndex = _cameraDropdown.choices.IndexOf(selectedCameraName);
        // OnCameraSelected?.Invoke(selectedIndex);
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
