using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This enum represents the type of collision titles to be displayed.
/// </summary>
[System.Serializable]
public class CollisionTitle
{
    public CollisionSceneType sceneType;
    public string collisionTitle;
}


/// <summary>
/// This class represents the application settings. 
/// </summary>
[CreateAssetMenu(fileName = "AppSettings", menuName = "Scriptable Object/AppSettings")]
public class AppSettings : ScriptableObject
{
  [Header("Scene Title Settings")]
  public string appName;
  public string appSubtitle;   
  public string satelliteGridTitle;
  public string satelliteGridSubtitle;
  [Tooltip("The title for collision types")]
  public List<CollisionTitle> collisionTitles = new List<CollisionTitle>();
}
