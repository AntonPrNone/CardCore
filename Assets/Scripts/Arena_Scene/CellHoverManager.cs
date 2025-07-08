using UnityEngine;
using UnityEngine.InputSystem;

public class CellHoverManager : MonoBehaviour
{
    private GameObject lastCell;

    void Update()
    {
        if (Mouse.current == null || Camera.main == null)
            return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject cell = hit.collider.gameObject;

            // Проверка по тегу
            if (cell.CompareTag("Cell"))
            {
                if (cell != lastCell)
                {
                    if (lastCell != null)
                        SetCellHighlight(lastCell, false);

                    SetCellHighlight(cell, true);
                    lastCell = cell;
                }
            }
            else
            {
                // Если навели на что-то не с тегом "Cell", убираем подсветку с предыдущей
                if (lastCell != null)
                {
                    SetCellHighlight(lastCell, false);
                    lastCell = null;
                }
            }
        }
        else
        {
            if (lastCell != null)
            {
                SetCellHighlight(lastCell, false);
                lastCell = null;
            }
        }
    }

    void SetCellHighlight(GameObject cell, bool state)
    {
        var renderer = cell.GetComponent<MeshRenderer>();
        if (renderer != null)
            renderer.enabled = state;
    }
}
