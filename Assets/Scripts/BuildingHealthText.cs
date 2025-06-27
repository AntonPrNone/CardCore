using UnityEngine;
using TMPro;

public class BuildingHealthText : MonoBehaviour
{
    private BuildingHealth buildingHealth;
    private TextMeshProUGUI text;

    void Awake()
    {
        // ���� ����� � ���� ��� �����
        text = GetComponentInChildren<TextMeshProUGUI>();

        // ���� ��������� �������� � �������� ���� (��������� �����)
        Transform current = transform;
        while (current != null)
        {
            buildingHealth = current.GetComponent<BuildingHealth>();
            if (buildingHealth != null)
                break;
            current = current.parent;
        }

        if (text == null)
        {
            Debug.LogWarning("TextMeshProUGUI �� ������");
            enabled = false;
        }

        if (buildingHealth == null)
        {
            Debug.LogWarning("BuildingHealth �� ������");
            enabled = false;
        }
    }

    void Update()
    {
        if (buildingHealth != null && text != null)
        {
            text.text = $"{Mathf.CeilToInt(buildingHealth.currentHealth)} / {Mathf.CeilToInt(buildingHealth.maxHealth)}";

            // ������������ � ������
            if (Camera.main != null)
                transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        }
    }
}
