using UnityEngine;

///<summary>
///Handles configurations for bindable buttons throughout the application.
///Keys are bound to their respective keycode, and accessed by a separate controller or manager.
///<summary>
[CreateAssetMenu(fileName = "KeybindSettings", menuName = "Settings/KeybindSettings")]
public class KeybindSettings : ScriptableObject
{
    public KeyCode pauseKey = KeyCode.Space;
    public KeyCode restartKey = KeyCode.R;
    public KeyCode increaseSpeedKey = KeyCode.UpArrow;
    public KeyCode decreaseSpeedKey = KeyCode.DownArrow;
    public KeyCode resetSpeedKey = KeyCode.Backspace;
    public KeyCode recordKey = KeyCode.T;
    public KeyCode saveKey = KeyCode.S;
    public KeyCode zenModeKey = KeyCode.Z;
    public KeyCode camera1Key = KeyCode.Alpha1;
    public KeyCode camera2Key = KeyCode.Alpha2;
    public KeyCode camera3Key = KeyCode.Alpha3;
    public KeyCode toggleKeybindPanel = KeyCode.K;
}