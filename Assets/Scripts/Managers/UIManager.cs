// Description: Manages persistent UI elements across scenes

using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Singleton class to manage persistent UI elements across scenes
/// </summary>
public class UIManager : MonoBehaviour
{
  private static UIManager _instance;
  public static UIManager Instance => _instance;

  // Persistent UI elements
  [SerializeField] private UIDocument _sharedUIDocument;

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
        
        // TODO: Initialize persistent UI elements
    }
}