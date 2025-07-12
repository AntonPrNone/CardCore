using UnityEngine;
using System;

public class MapSelectionManager : MonoBehaviour
{
    [Header("Map References")]
    [Tooltip("List of parent GameObjects for each map in the scene.")]
    [SerializeField] private GameObject[] maps;

    [Header("Spawning")]
    [Tooltip("Prefab of the character to spawn.")]
    [SerializeField] private GameObject characterPrefab;

    [Header("UI and Input")]
    [Tooltip("Reference to the map-selection UI canvas.")]
    [SerializeField] private MapSelectionUI mapSelectionUI;
    [Tooltip("Reference to the cell-click handler component.")]
    [SerializeField] private CellClickHandler cellClickHandler;

    private bool mapChosen = false;

    void Start()
    {
        // Initially hide all maps
        for (int i = 0; i < maps.Length; i++)
            maps[i].SetActive(false);

        // Ensure UI is visible and hooked up
        if (mapSelectionUI == null)
            Debug.LogError("MapSelectionUI reference is missing on MapSelectionManager.");
        else
            mapSelectionUI.gameObject.SetActive(true);

        // Disable clicking until a map is chosen
        if (cellClickHandler != null)
        {
            cellClickHandler.enabled = false;
            cellClickHandler.OnCellClicked += HandleCellClicked;
        }
        else
        {
            Debug.LogError("CellClickHandler reference is missing on MapSelectionManager.");
        }
    }

    /// <summary>
    /// Called by MapSelectionUI when a map button is pressed.
    /// </summary>
    public void SelectMap(int mapIndex)
    {
        if (mapIndex < 0 || mapIndex >= maps.Length)
        {
            Debug.LogError($"MapSelectionManager: Invalid map index {mapIndex}");
            return;
        }

        // Activate the chosen map, deactivate others
        for (int i = 0; i < maps.Length; i++)
            maps[i].SetActive(i == mapIndex);

        mapChosen = true;
        Debug.Log($"MapSelectionManager: Map {mapIndex} selected.");

        // Disable UI
        if (mapSelectionUI != null)
            mapSelectionUI.gameObject.SetActive(false);

        // Enable cell-clicking
        if (cellClickHandler != null)
            cellClickHandler.enabled = true;
    }

    private void HandleCellClicked(GameObject cell)
    {
        if (!mapChosen)
            return;

        if (characterPrefab == null)
        {
            Debug.LogError("MapSelectionManager: Character prefab is not assigned.");
            return;
        }

        // Spawn the character at the center of the cell
        Vector3 spawnPos = cell.transform.position;
        Quaternion spawnRot = Quaternion.identity;
        Instantiate(characterPrefab, spawnPos, spawnRot);
        Debug.Log($"MapSelectionManager: Spawned character at {spawnPos}");
    }

    private void OnDestroy()
    {
        if (cellClickHandler != null)
            cellClickHandler.OnCellClicked -= HandleCellClicked;
    }
}