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

        // Instantiate the satellite prefab
        instantiatedSatellite = Instantiate(selectedSatellite.satellitePrefab, satelliteContainer);

        // Apply scaling to the satellite prefab
        instantiatedSatellite.transform.localScale *= selectedSatellite.displayScale;

        // Apply rotation to the satellite prefab
        // instantiatedSatellite.transform.rotation = Quaternion.Euler(0, 180, 0);
    }

    private void OnDestroy()
    {
        if (instantiatedSatellite != null)
        {
            Destroy(instantiatedSatellite);
        }
    }
}
