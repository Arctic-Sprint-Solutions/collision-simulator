using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class representing a catalog of satellite Scriptable objects.
/// </summary>
[CreateAssetMenu(fileName = "SatelliteCatalog", menuName = "Scriptable Object/SatelliteCatalog")]
public class SatelliteCatalog : ScriptableObject
{
    public List<Satellite> satellites = new List<Satellite>();
}
