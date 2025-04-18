using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections;

// Controller for handling settings UI and key rebinding
public class SettingsController : MonoBehaviour
{
    public UIDocument uiDocument;
    private Button pauseButton;
    private Button restartButton;

    private bool waitingForKey = false;
    private Action<KeyCode> onKeySelected;

    private void Start()
    {
        // Offset start timing to ensure UI is ready
        StartCoroutine(InitUI());
    }

    private IEnumerator InitUI()
    {
        // Wait for InputManager to be ready
        while (InputManager.Instance == null || InputManager.Instance.keybinds == null)
        {
            yield return null;
        }
        // Fetch UI elements
        var root = uiDocument.rootVisualElement;

        pauseButton = root.Q<Button>("PauseButton");
        restartButton = root.Q<Button>("RestartButton");

        // Button presses triggers a rebind (key update) and UI update
        if (pauseButton != null)
        {
            pauseButton.clicked += () => StartKeyRebind("Pause");
            pauseButton.text = InputManager.Instance.keybinds.pauseKey.ToString();
        }
        else
        {
            Debug.LogError("PauseButton not found");
        }

        if (restartButton != null)
        {
            restartButton.clicked += () => StartKeyRebind("Restart");
            restartButton.text = InputManager.Instance.keybinds.restartKey.ToString();
        }
        else
        {
            Debug.LogError("RestartButton not found");
        }
    }

    private void Update()
    {
        if (!waitingForKey) return;

        foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(key))
            {
                waitingForKey = false;
                onKeySelected?.Invoke(key);
                break;
            }
        }
    }

    private void StartKeyRebind(string action)
    {
        if (InputManager.Instance == null || InputManager.Instance.keybinds == null)
        {
            Debug.LogError("InputManager or keybinds not initialized!");
            return;
        }

        waitingForKey = true;

        if (action == "Pause" && pauseButton != null)
        {
            pauseButton.text = "Press any key";
            onKeySelected = (key) =>
            {
                InputManager.Instance.keybinds.pauseKey = key;
                pauseButton.text = key.ToString();
            };
        }
        else if (action == "Restart" && restartButton != null)
        {
            restartButton.text = "Press any key";
            onKeySelected = (key) =>
            {
                InputManager.Instance.keybinds.restartKey = key;
                restartButton.text = key.ToString();
            };
        }
        else
        {
            Debug.LogError($"Unknown action '{action}' or button was null.");
        }
    }
}
