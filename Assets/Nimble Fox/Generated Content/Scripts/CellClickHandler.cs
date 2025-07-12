using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class CellClickHandler : MonoBehaviour
{
    /// <summary>
    /// Fired when the player successfully clicks on a valid cell.
    /// </summary>
    public event Action<GameObject> OnCellClicked;

    [Tooltip("Optional explicit camera reference; if null, Camera.main will be used.")]
    [SerializeField] private Camera mainCamera;

    void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
        if (mainCamera == null)
            Debug.LogError("CellClickHandler: No camera assigned and Camera.main is null.");
    }

    void Update()
    {
        if (!enabled)
            return;

        // Use new Input System API to detect left mouse button click
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (mainCamera == null)
                return;

            // Use new Input System API to get mouse position
            Vector2 mousePosition = Mouse.current.position.ReadValue();

            Ray ray = mainCamera.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                GameObject obj = hit.collider.gameObject;
                if (obj.CompareTag("Cell"))
                {
                    OnCellClicked?.Invoke(obj);
                }
            }
        }
    }
}