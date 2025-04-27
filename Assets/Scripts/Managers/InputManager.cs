using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    public KeybindSettings keybinds;
    private UIManager uiManager;
    private bool isCollisionScene = false;

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

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

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

    private void Update()
    {
        if (!isCollisionScene || keybinds == null) return;

        if (Input.GetKeyDown(keybinds.pauseKey))
        {
            Debug.Log("[InputManager] Pause key pressed");
            uiManager?.TogglePause();
        }

        if (Input.GetKeyDown(keybinds.restartKey))
        {
            Debug.Log("[InputManager] Restart key pressed");
            uiManager?.RestartScene();
        }

        if(Input.GetKeyDown(keybinds.recordKey))
        {
            Debug.Log("[InputManager] Record key pressed");
            VideoManager.Instance?.ToggleRecording();
        }

        if (Input.GetKeyDown(keybinds.saveKey))
        {
            Debug.Log("[InputManager] Save key pressed");
            VideoManager.Instance?.SaveCurrentRecording();
        }
    }
}