using UnityEngine;
using UnityEngine.UI;

public class UnitSelectButton : MonoBehaviour
{
    [Header("»ндекс юнита в массиве unitPrefabs у менеджера")]
    public int unitIndex;

    [Header("÷вет при выборе")]
    public Color selectedColor = Color.yellow;

    [Header("÷вет по умолчанию")]
    public Color normalColor = Color.white;

    [SerializeField] private CellHoverManager hoverManager;
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
        if (image != null)
            image.color = normalColor;
    }

    public void OnClick()
    {
        if (hoverManager == null)
        {
            Debug.LogWarning("CellHoverManager не назначен!");
            return;
        }

        hoverManager.SelectUnit(unitIndex, this);
    }

    public void SetSelected(bool isSelected)
    {
        if (image != null)
            image.color = isSelected ? selectedColor : normalColor;
    }

    public void SetManager(CellHoverManager mgr)
    {
        hoverManager = mgr;
    }
}
    