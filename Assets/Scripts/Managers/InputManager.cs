// Description: The InputManager class handles user input and triggers corresponding events
using UnityEngine;

/// <summary>
/// Singleton class to manage user input and trigger corresponding events.
/// </summary>
public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    [SerializeField] public KeybindSettings keybinds;

    #region Events
    public event System.Action OnTogglePause;
    public event System.Action OnRestartScene;
    public event System.Action OnIncreaseTimescale;
    public event System.Action OnDecreaseTimescale;
    public event System.Action OnResetTimescale;
    public event System.Action<int> OnCameraKeyPressed;
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
        if(Input.GetKeyDown(keybinds.camera1Key))
        {
            Debug.Log("[InputManager] Camera 1 key pressed");
            OnCameraKeyPressed?.Invoke(0);
        }
        if(Input.GetKeyDown(keybinds.camera2Key))
        {
            Debug.Log("[InputManager] Camera 2 key pressed");
            OnCameraKeyPressed?.Invoke(1);
        }
        if(Input.GetKeyDown(keybinds.camera3Key))
        {
            Debug.Log("[InputManager] Camera 3 key pressed");
            OnCameraKeyPressed?.Invoke(2);
        }
        if(Input.GetKeyDown(keybinds.toggleKeybindPanel))
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