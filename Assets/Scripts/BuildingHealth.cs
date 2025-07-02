using UnityEngine;
using TMPro;
using UnityEngine.Events;
using System.Collections;

public class BuildingHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 200f;
    public float currentHealth;

    [Header("UI")]
    public TextMeshProUGUI hpText;

    [Header("Events")]
    public UnityEvent onDeath;

    [Header("Dissolve Settings")]
    public float dissolveDuration = 1f; // время анимации расщепления

    private Material buildingMaterial;
    private static readonly int DissolveAmountID = Shader.PropertyToID("_DissolveAmount");

    public bool IsDead => currentHealth <= 0;

    void Awake()
    {
        currentHealth = maxHealth;

        if (hpText == null)
            hpText = GetComponentInChildren<TextMeshProUGUI>();

        UpdateHPText();

        // Получаем материал из рендера (предполагаем, что он один)
        var renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            // Лучше использовать instance материала, чтобы не менять глобальный
            buildingMaterial = renderer.material;
            buildingMaterial.SetFloat(DissolveAmountID, 0f);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0f);

        UpdateHPText();

        if (currentHealth <= 0)
        {
            StartCoroutine(DissolveAndDestroy());
        }
    }

    void UpdateHPText()
    {
        if (hpText != null)
            hpText.text = Mathf.Max(0, Mathf.CeilToInt(currentHealth)).ToString();
    }

    IEnumerator DissolveAndDestroy()
    {
        onDeath?.Invoke();

        if (buildingMaterial == null)
        {
            Destroy(gameObject);
            yield break;
        }

        float elapsed = 0f;

        while (elapsed < dissolveDuration)
        {
            elapsed += Time.deltaTime;
            float dissolveValue = Mathf.Clamp01(elapsed / dissolveDuration);
            buildingMaterial.SetFloat(DissolveAmountID, dissolveValue);
            yield return null;
        }

        Destroy(gameObject);
    }
}
