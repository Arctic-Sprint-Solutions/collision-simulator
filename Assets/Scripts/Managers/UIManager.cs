// Description: Manages persistent UI elementsOnCameraSelected across scenes

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using System.Collections.Generic;

/// <summary>
/// Singleton class to manage persistent UI elements across scenes
/// </summary>
public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance => _instance;

    // Persistent UI elements
    [SerializeField] private UIDocument _sharedUIDocument;
    [SerializeField] private VisualTreeAsset _SceneUIDocument;
    private VisualElement _root;
    private VisualElement _navBar;
    private VisualElement _backToMenuButton;
    private VisualElement _collisionUI;
    private Button _playPauseBtn;
    private Button _restartBtn;

    private bool isPaused = false;


    // Camera UI elements
    [SerializeField] private UIDocument _cameraManagerDocument;
    private VisualElement _rootCam;

    private DropdownField _cameraDropdown;
    private VisualElement _cameraDropdownUI;

    public delegate void CameraSelected(int index);
    public static event CameraSelected OnCameraSelected;

    /// <summary>
    /// Initializes the singleton instance and ensures that it persists across scenes
    /// </summary>
    private void Awake()
    {
        // Ensure singleton instance
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        InitializePersistentUI();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        CameraManager.OnCamerasUpdated += PopulateDropdown;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        CameraManager.OnCamerasUpdated -= PopulateDropdown;
    }



    /// <summary>
    /// A function for selecting which UI elements to display in each scene
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _collisionUI?.AddToClassList("d-none");
        _cameraDropdown?.AddToClassList("d-none");
        isPaused = false;
        Time.timeScale = 1f;


        // Sjekk om den nye scenen er merket som kollisjonsscene
        if (GameObject.FindWithTag("CollisionScene") != null)
        {
            _collisionUI?.RemoveFromClassList("d-none");
            _playPauseBtn.text = "Pause";
        }
        else
        {
            // Hide the dropdown in other scenes
            _cameraDropdown?.AddToClassList("d-none"); 
        }

    }





    /// <summary>
    /// Initializes the persistent UI elements
    /// </summary>
    private void InitializePersistentUI()
    {
        // Get the root visual element of the shared UI document
        _root = _sharedUIDocument.rootVisualElement;

        // Root for visual element of camera dropdown
        _rootCam = _cameraManagerDocument.rootVisualElement;







        // Get the NavBar and BackToMenuButton elements
        if (_navBar == null)
        {
            // Get the NavBar element from the shared UI document
            _navBar = _root.Q<VisualElement>("NavBar");
        }

        if (_backToMenuButton == null)
        {
            // Get the BackToMenuButton element from the shared UI document
            _backToMenuButton = _root.Q<VisualElement>("BackToMenuButton");
            // Register a callback for the button's click event
            _backToMenuButton.RegisterCallback<ClickEvent>(e => OnBackToMainMenuClicked());
        }

        if (_collisionUI == null)
        {
            InitializeCollisionUI();
            InitializeCameraDropDownUI();
        }

   


        // Hide the NavBar by default
        HideNavBar();
        HideDropdown();
    }

    private void InitializeCollisionUI()
    {
        _collisionUI = _root.Q<VisualElement>("CollisionUI");
        _collisionUI.RemoveFromClassList("d-none");
        // Finn knappene i det instansierte UI-et
        _playPauseBtn = _collisionUI.Q<Button>("playPauseButton");
        _restartBtn = _collisionUI.Q<Button>("restartButton");
        _playPauseBtn.text = "Pause";

        // Hide Unity default classes
        _playPauseBtn.RemoveFromClassList("unity-button");
        _playPauseBtn.RemoveFromClassList("unity-text-element");
        _restartBtn.RemoveFromClassList("unity-button");
        _restartBtn.RemoveFromClassList("unity-text-element");

        // Koble opp klikk-event til h�ndteringsfunksjoner
        _playPauseBtn.clicked += TogglePause;
        _restartBtn.clicked += RestartScene;
    }

    /// <summary>
    /// A function for initializing UI Dropdown element for camera
    /// </summary>


    private void InitializeCameraDropDownUI()
    {
        _cameraDropdownUI = _rootCam.Q<DropdownField>("CameraDropdown");
        _cameraDropdownUI.RemoveFromClassList("d-none");

    }

    /// <summary>
    /// A function for adding cameras in scene to dropdown selection
    /// </summary>
    private void PopulateDropdown(List<string> cameraNames)
    {
        if (_cameraDropdown == null) return;

        _cameraDropdown.choices = cameraNames;
        _cameraDropdown.value = cameraNames[0];
        _cameraDropdown.label = "Select Camera";
        _cameraDropdown.style.display = DisplayStyle.Flex;
    }

    /// <summary>
    /// Callback for the BackToMenuButton click event that loads the main menu scene
    /// </summary>
    private void OnBackToMainMenuClicked()
    {
        string buttonText = _backToMenuButton.Q<Label>().text;
        if(buttonText == "Go Back")
        {
            // Go back to the previous scene
            SimulationManager.Instance.LoadScene(SimulationManager.Instance.PreviousScene);
        } else 
        {
            // Load the main menu scene
            SimulationManager.Instance.LoadScene("MainMenu");
        }
    }

    /// <summary>
    /// Shows the NavBar element
    /// </summary>
    public void ShowNavBar(string backButtonText = "Main Menu")
    {
        if(_navBar != null)
        {
            _navBar.style.display = DisplayStyle.Flex;

            // Set the text of the BackToMenuButton
            if(_backToMenuButton != null)
            {
                _backToMenuButton.Q<Label>().text = backButtonText;
            }
        }
    }


    /// <summary>
    /// Hides the NavBar element
    /// </summary>
    public void HideNavBar()
    {
        if(_navBar != null)
        {
            _navBar.style.display = DisplayStyle.None;
        }

    }


    /// <summary>
    /// Shows the Drodown element
    /// </summary>
    public void ShowDropDown()
    {
        if (_cameraDropdown != null)
        {
            _cameraDropdown.style.display = DisplayStyle.Flex;
        }
    }

    /// <summary>
    // A Function for hiding drop down for camera selection
    /// </summary>
    public void HideDropdown()
    {
        if (_cameraDropdown != null)
        {
            _cameraDropdown.style.display = DisplayStyle.None;
        }
    }


    /// <summary>
    /// A function for pausing time in scene
    /// </summary>
    private void TogglePause()
    {
        // Toggle pause-status
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;

        _playPauseBtn.text = isPaused ? "Resume" : "Pause";
    }

    /// <summary>
    /// A function for restart/reloading scene
    /// </summary>
    private void RestartScene()
    {
        // Restart gjeldende scene
        isPaused = false;
        Time.timeScale = 1f;
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.name);
    }

    /// <summary>
    /// A function for destroying scene
    /// </summary>
    private void OnDestroy()
    {

        // Unregister the callback to avoid memory leaks
        if (_backToMenuButton != null)
        {
            _backToMenuButton.UnregisterCallback<ClickEvent>(e => OnBackToMainMenuClicked());
        }

        // Clean up references to avoid memory leaks
        _instance = null;
        _root = null;
        _navBar = null;
        _backToMenuButton = null;
        _sharedUIDocument = null;
        _cameraManagerDocument = null;

    }

}