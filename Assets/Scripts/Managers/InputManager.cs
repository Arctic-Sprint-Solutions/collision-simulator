using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

///<summary>
///Handles inputs based on the SceneTag.
///<summary>
public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    [SerializeField] public KeybindSettings keybinds;
    private UIManager uiManager;
    private bool isCollisionScene = false;

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
    ///Locates UIManager, and subscribes to the scene loaded event.
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

        uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager == null)
        {
            Debug.LogError("UIManager not found in the scene.");
            enabled = false;
            return;
        }



        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <summary>
    ///Unsubscribes from the scene loaded event when the object is destroyed.
    /// <summary>
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    ///When scene is loaded, collect SceneTag and handles which keybinds are availible. 
    ///Handles scene-specific logic when a new scene is loaded.
    /// <summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject tagObj = GameObject.Find("SceneTag");
        if (tagObj != null && tagObj.CompareTag("CollisionScene"))
        {
            isCollisionScene = true;
            Debug.Log($"[InputManager] Entered collision scene: {scene.name}");
        }
        else
        {
            isCollisionScene = false;
            Debug.Log($"[InputManager] Entered non-collision scene: {scene.name}");
        }
    }

    /// <summary>
    ///Monitors keypresses according to scene, and manages logic.
    /// <summary>
    private void Update()
    {
        if (keybinds == null) return;

        if (isCollisionScene)
        {
            HandleCollisionSceneInputs();
        }
        else
        {
            return;
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
            uiManager?.ToggleZenMode();
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
}