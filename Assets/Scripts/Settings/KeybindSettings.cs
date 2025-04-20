using UnityEngine;

[CreateAssetMenu(fileName = "KeybindSettings", menuName = "Settings/KeybindSettings")]
public class KeybindSettings : ScriptableObject
{
    public KeyCode pauseKey = KeyCode.Space;
    public KeyCode restartKey = KeyCode.R;
}