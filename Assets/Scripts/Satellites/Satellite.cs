using UnityEngine;


/// <summary>
/// This class represents a satellite and its properties.
/// </summary>
[CreateAssetMenu(fileName = "Satellite", menuName = "Scriptable Object/Satellite")]
public class Satellite : ScriptableObject
{
    [Header("Satellite Properties")]
    public string satelliteName; 
    public string type;
    public string leoInfo;
    public float weight;
    public int launchYear;
    [Tooltip("The prefab name used for the preview scene")]
    public string prefabName;
    [Tooltip("The 2D preview image to display in the UI")]
    public Sprite previewImage;
    [Header("Collision Scenes")]
    [Tooltip("Space debris scene name")]
    public string debrisSceneName;
    [Tooltip("Satellite collision scene name")]
    public string satelliteSceneName;
}
