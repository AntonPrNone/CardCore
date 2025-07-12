using UnityEngine;
using TMPro;

/// <summary>
/// Боевая логика рыцаря: атака, получение урона, управление здоровьем.
/// </summary>
[RequireComponent(typeof(Collider))]
public class KnightCombat : CombatEntity
{
    private KnightCard card;
    [SerializeField] private float currentHealth;
    private float cooldownTimer;

    private ICombatTarget targetCombat;
    private Transform targetTransform;

    private TextMeshProUGUI hpText;
    private bool wasInCombat = false;

    [Header("UI")]
    [SerializeField] private Color idleColor = Color.white;
    [SerializeField] private Color combatColor = Color.red;

    private const float MaxCooldownValue = float.MaxValue;

    public override bool IsDead => currentHealth <= 0f;

    void Start()
    {
        card = KnightCard.Template.Clone();
        currentHealth = card.MaxHealth;
        cooldownTimer = 0f;
        hpText = GetComponentInChildren<TextMeshProUGUI>();
        UpdateHPUI();
    }

    void Update()
    {
        if (IsDead) return;

        bool inCombat = false;
        if (targetTransform != null && targetCombat != null && !targetCombat.IsDead)
        {
            float surfaceDistance = CombatUtils.GetSurfaceDistance(transform, targetTransform);
            inCombat = surfaceDistance <= card.AttackRange;
        }

        if (inCombat != wasInCombat)
            wasInCombat = inCombat;

        if (hpText != null)
            hpText.color = inCombat ? combatColor : idleColor;

        if (!inCombat) return;

        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer <= 0f)
        {
            PerformAttack();
            cooldownTimer = card.AttackCooldown;
        }
    }

    public void SetTarget(Transform t)
    {
        targetTransform = t;
        targetCombat = t != null ? t.GetComponent<ICombatTarget>() : null;
    }

    private void PerformAttack()
    {
        if (targetCombat != null && !targetCombat.IsDead)
            CombatManager.Instance?.RegisterAttack(this, targetCombat, card.AttackDamage);
    }

    public float GetAttackRange()
    {
        card ??= KnightCard.Template.Clone();
        return card.AttackRange;
    }

    public bool InCombatWithTarget(Transform t)
    {
        if (t == null || targetTransform == null || targetCombat == null || targetCombat.IsDead)
            return false;
        if (t != targetTransform)
            return false;
        float surfaceDistance = CombatUtils.GetSurfaceDistance(transform, targetTransform);
        return surfaceDistance <= card.AttackRange;
    }

    public override void TakeDamage(float amount)
    {
        if (IsDead) return;
        currentHealth -= amount;
        UpdateHPUI();
        if (currentHealth <= 0f)
            Die();
    }

    private void UpdateHPUI()
    {
        if (hpText != null)
        {
            string current = Mathf.Max(0, currentHealth).ToString("0");
            string max = card.MaxHealth.ToString("0");
            hpText.text = $"{current}\n―\n{max}";
        }
    }

    private void Die()
    {
        cooldownTimer = MaxCooldownValue;
        if (TryGetComponent(out UnityEngine.AI.NavMeshAgent agent))
            agent.isStopped = true;
        targetCombat = null;
        targetTransform = null;
        Destroy(gameObject);
    }

    public override Transform GetTransform() => transform;

    void OnDrawGizmosSelected()
    {
        float range = card != null ? card.AttackRange : KnightCard.Template.AttackRange;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    void OnDrawGizmos()
    {
        if (targetTransform != null)
        {
            Collider colA = GetComponent<Collider>();
            Collider colB = targetTransform.GetComponent<Collider>();
            Vector3 from = transform.position;
            Vector3 to = targetTransform.position;
            if (colA != null && colB != null)
            {
                from = colA.ClosestPoint(colB.bounds.center);
                to = colB.ClosestPoint(from);
            }
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(from, to);
            float dist = Vector3.Distance(from, to);
            Vector3 mid = (from + to) * 0.5f;
#if UNITY_EDITOR
            UnityEditor.Handles.Label(mid + Vector3.up * 0.5f, dist.ToString("F2"));
#endif
        }
    }
}