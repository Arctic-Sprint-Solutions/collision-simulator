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
    [Tooltip("The name of the scene that contains this satellite's visualization")]
    public string satelliteSceneName;
    [Tooltip("The 2D preview image to display in the UI")]
    public Sprite previewImage;
    [Tooltip("The 3D model prefab")]
    public GameObject satellitePrefab;
}
