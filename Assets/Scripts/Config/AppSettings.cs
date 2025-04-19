using UnityEngine;

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
  [Tooltip("Space Debris Scene Title")]
  public string debrisSceneTitle;
  public string aboutSceneTitle;
  [Tooltip("Satellite Collision Scene Title")]
  public string satelliteSceneTitle;
}
