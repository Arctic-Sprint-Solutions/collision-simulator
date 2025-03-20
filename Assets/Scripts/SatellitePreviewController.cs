using UnityEngine;
using System.Collections;

public class SatellitePreviewController : MonoBehaviour
{
    [SerializeField] private Transform satelliteContainer;
    private GameObject instantiatedSatellite;

    private void Start()
    {
        LoadSelectedSatellite();   
    }

    private void LoadSelectedSatellite()
    {
        // Get the selected satellite from the SimulationManager
        Satellite selectedSatellite = SimulationManager.Instance.SelectedSatellite;
        
        if (selectedSatellite == null)
        {
            Debug.LogError("Selected satellite is null.");
            return;
        }

        if(selectedSatellite.satellitePrefab == null)
        {
            Debug.LogError("Satellite prefab is null.");
            return;
        }

        // Instantiate the satellite prefab into the satellite container
        instantiatedSatellite = Instantiate(selectedSatellite.satellitePrefab, satelliteContainer);
    }

    private void OnDestroy()
    {
        if (instantiatedSatellite != null)
        {
            Destroy(instantiatedSatellite);
        }
    }
}
