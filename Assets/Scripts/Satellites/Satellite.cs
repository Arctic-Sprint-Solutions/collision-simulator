using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

/// <summary>
/// Enum representing different types of collision scenes.
/// </summary>
public enum CollisionSceneType
{
    SpaceDebrisCollision,
    SatelliteCollision
}

/// <summary>
/// Represents a collision scene for a satellite.
/// Contains information about the scene asset and its type.
/// </summary>
[System.Serializable]
public class CollisionScene
{
    // public string sceneName;
    public SceneAsset sceneAsset;
    public CollisionSceneType sceneType;
}

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
    [Tooltip("The 2D preview image to display in the UI")]
    public Sprite previewImage;
    [Tooltip("The 3D model preview prefab")]
    public GameObject satellitePrefab;
    [Header("Collision Scenes")]
    [Tooltip("Scene for space debris collision")]
    public CollisionScene debrisCollision = new CollisionScene { sceneType = CollisionSceneType.SpaceDebrisCollision };
    [Tooltip("Scene for satellite collision")]
    public CollisionScene satelliteCollision = new CollisionScene { sceneType = CollisionSceneType.SatelliteCollision };
}

