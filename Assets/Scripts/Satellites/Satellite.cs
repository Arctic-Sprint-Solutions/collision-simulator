using UnityEngine;


/// <summary>
/// This class represents a satellite and its properties.
/// </summary>
[CreateAssetMenu(fileName = "Satellite", menuName = "Scriptable Object/Satellite")]
public class Satellite : ScriptableObject
{
    [Header("Satellite Properties")]
    public string satelliteName; 
    [Tooltip("The satllite's purpose")]
    public string type;
    [Tooltip("The satellite's low earth orbit (LEO) information in km")]
    public string leoInfo;
    [Tooltip("The satellite's weight in kilograms")]
    public float weight;
    [Tooltip("The satellite's launch year")]
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
