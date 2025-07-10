using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class TowerCombat : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 200f;
    public float attackDamage = 15f;
    public float attackSpeed = 1f; // атак в секунду

    [Header("Dissolve Settings")]
    public float dissolveDuration = 1f;
    private static readonly int DissolveAmountID = Shader.PropertyToID("_DissolveAmount");

    [Header("Events")]
    public UnityEvent onDeath;

    [Header("UI")]
    public TextMeshProUGUI hpText;

    [Header("UI Colors")]
    public Color idleColor = Color.white;
    public Color combatColor = Color.red;

    private float currentHealth;
    private Transform target;
    private bool inCombat;
    private Material dissolveMaterial;

    public bool IsDead => currentHealth <= 0;

    void Start()
    {
        currentHealth = maxHealth;

        if (hpText == null)
            hpText = GetComponentInChildren<TextMeshProUGUI>();

        if (TryGetComponent(out Renderer renderer))
            dissolveMaterial = renderer.material;

        UpdateHPText();
    }

    void Update()
    {
        // Атака, если есть цель
        if (target != null && !inCombat && !IsTargetDead())
        {
            StartCoroutine(AttackRoutine());
        }

        // Обновление цвета текста
        if (hpText != null)
        {
            bool inCombatNow = target != null && !IsTargetDead();
            Color desiredColor = inCombatNow ? combatColor : idleColor;

            if (hpText.color != desiredColor)
                hpText.color = desiredColor;
        }
    }

    public void TakeDamage(float damage, Transform attacker = null)
    {
        if (IsDead)
        {
            Debug.Log($"{name}: Башня уже мертва");
            return;
        }

        Debug.Log($"{name}: Получает урон {damage}");

        currentHealth -= damage;
        UpdateHPText();

        // Если attacker указан, установить его как цель атаки
        if (attacker != null && target != attacker)
        {
            target = attacker;
        }

        if (currentHealth <= 0)
        {
            StartCoroutine(DissolveAndDie());
        }
    }


    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    IEnumerator AttackRoutine()
    {
        inCombat = true;

        while (target != null && !IsDead && !IsTargetDead())
        {
            // Пытаемся нанести урон
            var unitCombat = target.GetComponent<UnitCombat>();
            if (unitCombat != null)
            {
                unitCombat.TakeDamage(attackDamage);
            }
            else
            {
                // Можем добавить логику для других типов цели
                break;
            }

            yield return new WaitForSeconds(1f / attackSpeed);
        }

        inCombat = false;
    }

    public void TakeDamage(float damage)
    {
        if (IsDead)
        {
            Debug.Log($"{name}: Башня уже мертва");
            return;
        }

        Debug.Log($"{name}: Получает урон {damage}");

        currentHealth -= damage;
        UpdateHPText();

        if (currentHealth <= 0)
        {
            StartCoroutine(DissolveAndDie());
        }
    }

    IEnumerator DissolveAndDie()
    {
        onDeath?.Invoke();

        if (dissolveMaterial == null)
        {
            Destroy(gameObject);
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < dissolveDuration)
        {
            elapsed += Time.deltaTime;
            float dissolveValue = Mathf.Clamp01(elapsed / dissolveDuration);
            dissolveMaterial.SetFloat(DissolveAmountID, dissolveValue);
            yield return null;
        }

        Destroy(gameObject);
    }

    bool IsTargetDead()
    {
        if (target == null) return true;

        var combat = target.GetComponent<UnitCombat>();
        return combat == null || combat.IsDead;
    }

    void UpdateHPText()
    {
        if (hpText != null)
        {
            string current = Mathf.Max(0, currentHealth).ToString("0");
            string max = maxHealth.ToString("0");
            hpText.text = $"{current}\n―\n{max}";
        }
    }
}
