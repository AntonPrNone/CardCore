using UnityEngine;
using UnityEngine.InputSystem;

public class CellHoverManager : MonoBehaviour
{
    [Header("Боевые юниты (для спавна)")]
    public GameObject[] unitPrefabs;

    [Header("Призрачные юниты (для предпросмотра)")]
    public GameObject[] previewPrefabs;

    [Header("UI-кнопки для выбора")]
    public UnitSelectButton[] unitButtons;

    [Header("Высота призрака над клеткой")]
    public float previewYOffset = 1f;

    private GameObject lastCell;
    private int selectedUnitIndex = -1;
    private UnitSelectButton selectedButton;
    private GameObject previewInstance;

    void Update()
    {
        if (Mouse.current == null || Camera.main == null)
            return;

        HandleHotkeys();

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject hitObj = hit.collider.gameObject;

            if (hitObj.CompareTag("Cell"))
            {
                if (hitObj != lastCell)
                {
                    if (lastCell != null)
                        SetCellHighlight(lastCell, false);

                    SetCellHighlight(hitObj, true);
                    lastCell = hitObj;

                    ShowPreview(lastCell.transform.position);
                }

                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    TrySpawnUnit(lastCell.transform.position);
                }
            }
            else
            {
                ClearHighlight();
            }
        }
        else
        {
            ClearHighlight();
        }

        if (previewInstance != null && lastCell != null)
        {
            Vector3 pos = lastCell.transform.position + Vector3.up * previewYOffset;
            previewInstance.transform.position = pos;
        }
    }

    void SetCellHighlight(GameObject cell, bool state)
    {
        var renderer = cell.GetComponent<MeshRenderer>();
        if (renderer != null)
            renderer.enabled = state;
    }

    void ShowPreview(Vector3 basePosition)
    {
        if (selectedUnitIndex < 0 || selectedUnitIndex >= previewPrefabs.Length)
            return;

        Vector3 spawnPos = basePosition + Vector3.up * previewYOffset;

        if (previewInstance == null)
        {
            previewInstance = Instantiate(previewPrefabs[selectedUnitIndex], spawnPos, Quaternion.identity);
        }
        else
        {
            previewInstance.transform.position = spawnPos;
        }
    }

    void ClearHighlight()
    {
        if (lastCell != null)
        {
            SetCellHighlight(lastCell, false);
            lastCell = null;
        }
    }

    void ClearPreview()
    {
        if (previewInstance != null)
        {
            Destroy(previewInstance);
            previewInstance = null;
        }
    }

    public void SelectUnit(int index, UnitSelectButton button)
    {
        if (selectedButton != null)
            selectedButton.SetSelected(false);

        selectedUnitIndex = index;
        selectedButton = button;
        selectedButton.SetSelected(true);

        ClearPreview();

        if (lastCell != null)
            ShowPreview(lastCell.transform.position);
    }

    private void TrySpawnUnit(Vector3 position)
    {
        if (selectedUnitIndex < 0 || selectedUnitIndex >= unitPrefabs.Length)
            return;

        position.y = 1f;
        Instantiate(unitPrefabs[selectedUnitIndex], position, Quaternion.identity);
        ClearPreview();

        DeselectUnit();
    }

    public void DeselectUnit()
    {
        if (selectedButton != null)
        {
            selectedButton.SetSelected(false);
            selectedButton = null;
        }

        selectedUnitIndex = -1;
        ClearPreview();
    }

    private void HandleHotkeys()
    {
        if (Keyboard.current == null) return;

        // Проверяем от 1 до unitButtons.Length
        if (unitButtons.Length >= 1 && Keyboard.current.digit1Key.wasPressedThisFrame)
            SelectUnit(0, unitButtons[0]);

        if (unitButtons.Length >= 2 && Keyboard.current.digit2Key.wasPressedThisFrame)
            SelectUnit(1, unitButtons[1]);

        if (unitButtons.Length >= 3 && Keyboard.current.digit3Key.wasPressedThisFrame)
            SelectUnit(2, unitButtons[2]);
    }

}
