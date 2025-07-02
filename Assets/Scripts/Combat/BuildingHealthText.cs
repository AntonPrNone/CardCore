using UnityEngine;
using TMPro;

public class BuildingHealthText : MonoBehaviour
{
    private BuildingCombat buildingCombat;
    private TextMeshProUGUI text;

    void Awake()
    {
        // ���� ����� � ���� ��� �����
        text = GetComponentInChildren<TextMeshProUGUI>();

        // ���� ��������� �������� � �������� ���� (��������� �����)
        Transform current = transform;
        while (current != null)
        {
            buildingCombat = current.GetComponent<BuildingCombat>();
            if (buildingCombat != null)
                break;
            current = current.parent;
        }

        if (text == null)
        {
            Debug.LogWarning("TextMeshProUGUI �� ������");
            enabled = false;
        }

        if (buildingCombat == null)
        {
            Debug.LogWarning("BuildingHealth �� ������");
            enabled = false;
        }
    }

    void Update()
    {
        if (buildingCombat != null && text != null)
        {
            text.text = $"{Mathf.CeilToInt(buildingCombat.currentHealth)} / {Mathf.CeilToInt(buildingCombat.maxHealth)}";

            // ������������ � ������
            if (Camera.main != null)
                transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        }
    }
}
