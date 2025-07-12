using UnityEngine;

/// <summary>
/// Provides UI for the player to select a map.
/// On map button click, notifies MapSelectionManager of the selected map.
/// Disables itself after selection.
/// </summary>
public class MapSelectionUI : MonoBehaviour
{
    [Tooltip("Reference to the MapSelectionManager in the scene.")]
    [SerializeField] private MapSelectionManager mapSelectionManager;

    /// <summary>
    /// Hook this method up in the UI Button OnClick and pass in the map index.
    /// </summary>
    public void OnMapButtonClick(int mapIndex)
    {
        if (mapSelectionManager != null)
        {
            mapSelectionManager.SelectMap(mapIndex);
            gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("MapSelectionUI: MapSelectionManager reference is not assigned.");
        }
    }
}