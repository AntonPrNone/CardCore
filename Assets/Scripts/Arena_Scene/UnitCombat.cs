using UnityEngine;
using TMPro;

public class UnitCombat : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 100f;
    public float attackRange = 2f;
    public float attackCooldown = 1f;
    public float damage = 10f;

    private float currentHealth;
    private float cooldownTimer;

    private Transform targetTransform;
    private UnitCombat targetCombat;

    private TextMeshProUGUI hpText;

    public bool IsDead => currentHealth <= 0f;

    private bool wasInCombat = false;

    private TowerCombat targetTowerCombat;



    [Header("UI")]
    public Color idleColor = Color.white;
    public Color combatColor = Color.red;

    void Start()
    {
        currentHealth = maxHealth;
        cooldownTimer = 0f;

        // Find HP UI text if present
        hpText = GetComponentInChildren<TextMeshProUGUI>();
        UpdateHPUI();
    }

    void Update()
    {
        // Если юнит мёртв — ничего не делаем
        if (IsDead)
            return;

        // Проверка, жива ли цель
        bool inCombat = targetTransform != null &&
                       ((targetCombat != null && !targetCombat.IsDead) ||
                        (targetTowerCombat != null && !targetTowerCombat.IsDead));


        // Лог при входе или выходе из боя
        if (inCombat != wasInCombat)
        {
            if (inCombat)
                Debug.Log($"{name} вступил в бой с {targetTransform.name}");
            else
                Debug.Log($"{name} вышел из боя");

            wasInCombat = inCombat;
        }

        // Обновление цвета HP текста
        if (hpText != null)
        {
            hpText.color = inCombat ? combatColor : idleColor;
        }

        // Если не в бою — ничего не делаем
        if (!inCombat)
            return;

        // Отсчёт кулдауна
        cooldownTimer -= Time.deltaTime;

        // Проверка дистанции до цели и атака
        Collider targetCollider = targetTransform.GetComponent<Collider>();
        Vector3 closestPoint = targetCollider.ClosestPoint(transform.position);
        float dist = Vector3.Distance(transform.position, closestPoint);

        if (dist <= attackRange && cooldownTimer <= 0f)
        {
            PerformAttack();
            cooldownTimer = attackCooldown;
        }
    }


    /// <summary>
    /// Called by UnitMovement to assign a new target.
    /// </summary>
    /// <param name="t">Transform of the new target.</param>
    public void SetTarget(Transform t)
    {
        targetTransform = t;

        if (t != null)
        {
            targetCombat = t.GetComponent<UnitCombat>();
            targetTowerCombat = t.GetComponent<TowerCombat>(); // добавим это

            if (targetCombat == null && targetTowerCombat == null)
            {
                Debug.LogWarning($"{name}: Target has no combat component.");
            }
        }
        else
        {
            targetCombat = null;
            targetTowerCombat = null;
        }
    }


    void PerformAttack()
    {
        if (targetCombat != null && !targetCombat.IsDead && CombatManager.Instance != null)
        {
            CombatManager.Instance.RegisterAttack(this, targetCombat, damage);
        }
        else if (targetTowerCombat != null && !targetTowerCombat.IsDead)
        {
            Debug.Log($"{name} атакует башню {targetTowerCombat.name} на {damage} урона");
            targetTowerCombat.TakeDamage(damage, transform);
        }
    }




    /// <summary>
    /// Called by CombatManager when this unit takes damage.
    /// </summary>
    /// <param name="amount">Amount of damage to apply.</param>
    public void TakeDamage(float amount)
    {
        if (IsDead) return;

        currentHealth -= amount;
        UpdateHPUI();

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void UpdateHPUI()
    {
        if (hpText == null) return;

        string current = Mathf.Max(0, currentHealth).ToString("0");
        string max = maxHealth.ToString("0");

        hpText.text = $"{current}\n―\n{max}";
    }


    void Die()
    {
        // Stop any further actions
        cooldownTimer = float.MaxValue;
        // Optionally play death animation or disable movement
        var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null) agent.isStopped = true;

        targetCombat = null;
        targetTowerCombat = null;


        // Disable this component to stop Update()
        Destroy(gameObject);
    }
}