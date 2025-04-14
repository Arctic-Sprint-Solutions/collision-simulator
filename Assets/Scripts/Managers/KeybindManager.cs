using UnityEngine;
using System.Collections.Generic;

public class KeybindManager : MonoBehaviour
{
    public static KeybindManager Instance;


    private Dictionary<string, KeyCode> keybinds = new Dictionary<string, KeyCode>();
    private string[] actions = 
    {
        "Restart",
        "Pause",
    };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadKeybinds();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public KeyCode GetKey(string action)
    {
        return keybinds.ContainsKey(action) ? keybinds[action] : KeyCode.None;
    }

    public void SetKey(string action, KeyCode newKey)
    {
        keybinds[action] = newKey;
        PlayerPrefs.SetString(action, newKey.ToString());
        PlayerPrefs.Save();
    }

    private void LoadKeybinds()
    {
        keybinds["Restart"] = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Restart", "R"));
        keybinds["Pause"] = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Pause", "Space"));
    }
}
