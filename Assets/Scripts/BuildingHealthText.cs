using UnityEngine;
using TMPro;

public class BuildingHealthText : MonoBehaviour
{
    private BuildingHealth buildingHealth;
    private TextMeshProUGUI text;

    void Awake()
    {
        // Ищем текст в себе или детях
        text = GetComponentInChildren<TextMeshProUGUI>();

        // Ищем компонент здоровья у родителя выше (владельца башни)
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
            Debug.LogWarning("TextMeshProUGUI не найден");
            enabled = false;
        }

        if (buildingHealth == null)
        {
            Debug.LogWarning("BuildingHealth не найден");
            enabled = false;
        }
    }

    void Update()
    {
        if (buildingHealth != null && text != null)
        {
            text.text = $"{Mathf.CeilToInt(buildingHealth.currentHealth)} / {Mathf.CeilToInt(buildingHealth.maxHealth)}";

            // Поворачиваем к камере
            if (Camera.main != null)
                transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        }
    }
}
