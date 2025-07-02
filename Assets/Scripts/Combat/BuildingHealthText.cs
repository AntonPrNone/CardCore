using UnityEngine;
using TMPro;

public class BuildingHealthText : MonoBehaviour
{
    private BuildingCombat buildingCombat;
    private TextMeshProUGUI text;

    void Awake()
    {
        // Ищем текст в себе или детях
        text = GetComponentInChildren<TextMeshProUGUI>();

        // Ищем компонент здоровья у родителя выше (владельца башни)
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
            Debug.LogWarning("TextMeshProUGUI не найден");
            enabled = false;
        }

        if (buildingCombat == null)
        {
            Debug.LogWarning("BuildingHealth не найден");
            enabled = false;
        }
    }

    void Update()
    {
        if (buildingCombat != null && text != null)
        {
            text.text = $"{Mathf.CeilToInt(buildingCombat.currentHealth)} / {Mathf.CeilToInt(buildingCombat.maxHealth)}";

            // Поворачиваем к камере
            if (Camera.main != null)
                transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        }
    }
}
