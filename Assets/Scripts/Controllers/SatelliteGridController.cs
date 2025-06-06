// Description: This script manages the satellite grid UI, populating it with satellite cards from the catalog.

using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Localization.Tables;
using UnityEngine.Localization.Settings;
using System.Threading.Tasks;

/// <summary>
/// Controller for the satellite grid UI.
/// Sets up the grid and populates it with satellite cards from the catalog.
/// Each card displays satellite information and a button to select the satellite.
/// </summary>
public class SatelliteGridController : MonoBehaviour
{
    #region Properties
    /// <summary>
    /// Reference to the satellite catalog ScriptableObject.
    /// </summary>
    [SerializeField] private SatelliteCatalog satelliteCatalog;
    /// <summary>
    /// Reference to the UIDocument component for accessing UI elements.
    /// </summary>
    private UIDocument uiDocument;
    /// <summary>
    /// Reference to the StringTable for localization.
    /// </summary>
    private StringTable _stringTable;
    /// <summary>
    /// Reference to the VisualElement that contains the satellite grid.
    /// </summary>
    private VisualElement satelliteGridContainer;
    /// <summary>
    /// Reference to the root visual element of the UI document.
    /// </summary>
    private VisualElement _rootElement;
    #endregion

    /// <summary>
    /// Setup the satellite grid when the script is enabled.
    /// </summary>
    private void OnEnable()
    {
        SetupAsync();
    }

    /// <summary>
    /// Asynchronous setup method for the satellite grid.
    /// This method gets the UIDocument, initializes localization, and sets up the satellite grid.
    /// </summary>
    private async void SetupAsync()
    {
        // Normal startup logic here
        uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null)
        {
            Debug.LogError("UIDocument component not found on the GameObject.");
            return;
        }

        _rootElement = uiDocument.rootVisualElement;
        if (_rootElement == null)
        {
            Debug.LogError("Root element is null. Ensure the UIDocument is properly set up.");
            return;
        }

        await LocalizationSettings.InitializationOperation.Task;

        await LoadStringTableAsync();

        SetupSatelliteGrid();
    }

    /// <summary>
    /// Asynchronously load the StringTable for localization.
    /// </summary>
    private async Task LoadStringTableAsync()
    {
        var handle = LocalizationSettings.StringDatabase.GetTableAsync("UIStrings");
        await handle.Task;
        _stringTable = handle.Result;
    }

    /// <summary>
    /// Setup the satellite grid UI.
    /// This method clears the existing content in the grid and populates it with satellite cards.
    /// </summary>
    private void SetupSatelliteGrid()
    {
        // Find the satellite grid container in the UI
        satelliteGridContainer = _rootElement.Q<VisualElement>("SatelliteGridContainer");
        if (satelliteGridContainer == null)
        {
            Debug.LogError("SatelliteGridContainer not found in the UI.");
            return;
        }

        // Clear the existing content in the container
        satelliteGridContainer.Clear();

        if (satelliteCatalog == null)
        {
            Debug.LogError("SatelliteCatalog is not assigned.");
            return;
        }

        // Loop through each satellite in the catalog and create a card for it
        foreach (var satellite in satelliteCatalog.satellites)
        {
            CreateSatelliteCard(satellite);
        }
    }

    /// <summary>
    /// Create a VisualElement card for a satellite.
    /// This card displays the satellite's name, type, image, and information.
    /// It also includes a button to select the satellite.
    /// </summary>
    private void CreateSatelliteCard(Satellite satellite)
    {
        // Create a new card element
        var card = new VisualElement();
        card.AddToClassList("satellite-card");

        var titleContainer = new VisualElement();
        titleContainer.AddToClassList("satellite-info-container");

        // Satellite name and type labels
        var nameLabel = new Label(satellite.satelliteName);
        nameLabel.AddToClassList("satellite-title");
        titleContainer.Add(nameLabel);

        var typeLabel = new Label(satellite.type);
        typeLabel.AddToClassList("satellite-subtitle");
        titleContainer.Add(typeLabel);

        card.Add(titleContainer);

        // Satellite image
        var imageContainer = new VisualElement();
        imageContainer.AddToClassList("satellite-image-container");
        var satelliteImage = new Image();
        satelliteImage.AddToClassList("satellite-image");

        if (satellite.previewImage != null)
        {
            // Set the image source to the satellite's preview image
            satelliteImage.image = satellite.previewImage.texture;
        }
        else
        {
            Debug.Log($"Satellite image for {satellite.satelliteName} is missing.");
            satelliteImage.AddToClassList("satellite-image-missing");
        }

        imageContainer.Add(satelliteImage);
        card.Add(imageContainer);

        var infoAndButtonContainer = new VisualElement();
        infoAndButtonContainer.AddToClassList("satellite-info-and-button");

        // Satellite info elements
        var infoContainer = new VisualElement();
        infoContainer.AddToClassList("satellite-info-container");

        var leoLabel = new Label(LocalizedFormat("LEO_Label", satellite.leoInfo));
        leoLabel.AddToClassList("satellite-info-text");
        infoContainer.Add(leoLabel);

        var weightLabel = new Label(LocalizedFormat("Weight_Label", satellite.weight));
        weightLabel.AddToClassList("satellite-info-text");
        infoContainer.Add(weightLabel);

        var launchYearLabel = new Label(LocalizedFormat("LaunchYear_Label", satellite.launchYear));
        launchYearLabel.AddToClassList("satellite-info-text");
        infoContainer.Add(launchYearLabel);

        infoAndButtonContainer.Add(infoContainer);


        // Create select button
        var selectbutton = new Button(() => OnSatelliteSelected(satellite));
        selectbutton.text = Localized("Select_Label");
        selectbutton.RemoveFromClassList("unity-button");
        selectbutton.RemoveFromClassList("unity-text-element");
        selectbutton.AddToClassList("satellite-select-button");
        infoAndButtonContainer.Add(selectbutton);

        card.Add(infoAndButtonContainer);

        // Add the card to the grid
        satelliteGridContainer.Add(card);
    }

    /// <summary>
    /// Call the SimulationManager to load the selected satellite's scene.
    /// </summary>
    /// <param name="satellite">The selected satellite.</param>
    private void OnSatelliteSelected(Satellite satellite)
    {
        Debug.Log($"Satellite selected: {satellite.satelliteName}");
        SimulationManager.Instance.SelectSatellite(satellite);
    }

    /// <summary>
    /// Localizes a string using the StringTable.
    /// </summary>
    /// <param name="key">The key for the string in the StringTable.</param>
    /// <returns>The localized string.</returns>
    private string Localized(string key)
    {
        return _stringTable?.GetEntry(key)?.GetLocalizedString() ?? key;
    }

    /// <summary>
    /// Localizes a string using the StringTable with parameters.
    /// </summary>
    /// <param name="key">The key for the string in the StringTable.</param>
    /// <param name="args">The parameters to format the string with.</param>
    /// <returns>The localized string.</returns>
    private string LocalizedFormat(string key, params object[] args)
    {
        var entry = _stringTable?.GetEntry(key);
        if (entry == null)
        {
            return $"{key}: {string.Join(", ", args)}";
        }

        return entry.GetLocalizedString(args);
    }
}