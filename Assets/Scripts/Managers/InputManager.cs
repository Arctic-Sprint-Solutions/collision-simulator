using UnityEngine;

/// <summary>
/// Singleton class to manage input events and the logic for the keybinds.
/// <summary>
public class InputManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of InputManager
    /// </summary>
    public static InputManager Instance { get; private set; }
    /// <summary>
    /// Reference to the KeybindSettings scriptable object for managing keybinds
    /// </summary>
    [SerializeField] public KeybindSettings keybinds;

    #region Events
    /// <summary>
    /// Event triggered when the pause key is pressed
    /// </summary>
    public event System.Action OnTogglePause;
    /// <summary>
    /// Event triggered when the restart key is pressed
    /// </summary>
    public event System.Action OnRestartScene;
    /// <summary>
    /// Event triggered when the increase speed key is pressed
    /// </summary>
    public event System.Action OnIncreaseTimescale;
    /// <summary>
    /// Event triggered when the decrease speed key is pressed
    /// </summary>
    public event System.Action OnDecreaseTimescale;
    /// <summary>
    /// Event triggered when the reset speed key is pressed
    /// </summary>
    public event System.Action OnResetTimescale;
    /// <summary>
    /// Event triggered when a camera key is pressed
    /// Takes an integer parameter to identify the camera
    /// </summary>
    public event System.Action<int> OnCameraKeyPressed;
    /// <summary>
    /// Event triggered when the toggle keybind panel key is pressed
    /// </summary>
    public event System.Action OnToggleKeybindPanel;
    #endregion

    /// <summary>
    /// Sets up the singleton instance and ensures that only one instance of InputManager exists.
    /// <summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Update is called once per frame. Checks if the current scene is a collision scene 
    /// and calls the collision scene input handler.
    /// </summary>
    private void Update()
    {
        if (keybinds == null) return;

        // Check if the current scene is a collision scene
        if (SimulationManager.Instance?.IsCollisionScene == true)
        {
            HandleCollisionSceneInputs();
        }
    }

    /// <summary>
    /// Handles input specific to collision scenes (pausing, restarting, etc.).
    /// </summary>
    private void HandleCollisionSceneInputs()
    {
        if (Input.GetKeyDown(keybinds.pauseKey))
        {
            Debug.Log("[InputManager] Pause key pressed");
            OnTogglePause?.Invoke();
        }

        if (Input.GetKeyDown(keybinds.restartKey))
        {
            Debug.Log("[InputManager] Restart key pressed");
            OnRestartScene?.Invoke();
        }

        if (Input.GetKeyDown(keybinds.increaseSpeedKey))
        {
            Debug.Log("[InputManager] Increase speed key pressed");
            OnIncreaseTimescale?.Invoke();
        }

        if (Input.GetKeyDown(keybinds.decreaseSpeedKey))
        {
            Debug.Log("[InputManager] Decrease speed key pressed");
            OnDecreaseTimescale?.Invoke();
        }

        if (Input.GetKeyDown(keybinds.resetSpeedKey))
        {
            Debug.Log("[InputManager] Reset speed key pressed");
            OnResetTimescale?.Invoke();
        }

        if (Input.GetKeyDown(keybinds.recordKey))
        {
            Debug.Log("[InputManager] Record key pressed");
            VideoManager.Instance?.ToggleRecording();
        }

        if (Input.GetKeyDown(keybinds.saveKey))
        {
            Debug.Log("[InputManager] Save key pressed");
            VideoManager.Instance?.SaveCurrentRecording();
        }

        if (Input.GetKeyDown(keybinds.zenModeKey))
        {
            Debug.Log("[InputManager] Zen mode key pressed");
            UIManager.Instance?.ToggleZenMode();
        }
        if (Input.GetKeyDown(keybinds.camera1Key))
        {
            Debug.Log("[InputManager] Camera 1 key pressed");
            OnCameraKeyPressed?.Invoke(0);
        }
        if (Input.GetKeyDown(keybinds.camera2Key))
        {
            Debug.Log("[InputManager] Camera 2 key pressed");
            OnCameraKeyPressed?.Invoke(1);
        }
        if (Input.GetKeyDown(keybinds.camera3Key))
        {
            Debug.Log("[InputManager] Camera 3 key pressed");
            OnCameraKeyPressed?.Invoke(2);
        }
        if (Input.GetKeyDown(keybinds.toggleKeybindPanel))
        {
            Debug.Log("[InputManager] Toggle keybind panel key pressed");
            OnToggleKeybindPanel?.Invoke();
        }
    }

    /// <summary>
    /// Cleans up the singleton instance when the object is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}