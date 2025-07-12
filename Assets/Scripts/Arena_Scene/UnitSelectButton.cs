using UnityEngine;
using UnityEngine.UI;

public class UnitSelectButton : MonoBehaviour
{
    [Header("������ ����� � ������� unitPrefabs � ���������")]
    public int unitIndex;

    [Header("���� ��� ������")]
    public Color selectedColor = Color.yellow;

    [Header("���� �� ���������")]
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
            Debug.LogWarning("CellHoverManager �� ��������!");
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
    