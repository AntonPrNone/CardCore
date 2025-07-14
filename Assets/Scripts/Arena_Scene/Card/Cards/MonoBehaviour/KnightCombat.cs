using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider))]
public class KnightCombat : CombatEntity
{
    [Header("Статы")]
    [SerializeField] internal KnightCard card;
    [SerializeField] private float currentHealth;
    private float cooldownTimer = 0f;

    [Header("Цель")]
    private ICombatTarget targetCombat;
    private Transform targetTransform;

    [Header("UI")]
    [SerializeField] private Color idleColor = Color.white;
    [SerializeField] private Color combatColor = Color.red;
    private TextMeshProUGUI hpText;
    private bool wasInCombat = false;

    private const float MaxCooldownValue = float.MaxValue;

    public override bool IsDead => currentHealth <= 0f;

    // --- Unity lifecycle ---
    private void Start()
    {
        currentHealth = card != null ? card.MaxHealth : KnightCard.Template.MaxHealth;
        hpText = GetComponentInChildren<TextMeshProUGUI>();
        UpdateHPUI();
    }

    private void Update()
    {
        if (IsDead) return;

        UpdateCombatState();
        UpdateUI();

        if (!wasInCombat) return;

        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer <= 0f)
        {
            PerformAttack();
            cooldownTimer = card.AttackCooldown;
        }
    }

    // --- Combat logic ---
    private void UpdateCombatState()
    {
        bool inCombat = false;

        if (targetTransform != null && targetCombat != null && !targetCombat.IsDead)
        {
            if (IsValidEnemy(targetCombat))
            {
                float distance = CombatUtils.GetSurfaceDistance(transform, targetTransform);
                inCombat = distance <= card.AttackRange;
            }
            else
            {
                ClearTarget();
            }
        }

        if (inCombat != wasInCombat)
            wasInCombat = inCombat;
    }

    private void PerformAttack()
    {
        if (targetCombat == null || targetCombat.IsDead) return;
        if (IsValidEnemy(targetCombat))
        {
            CombatManager.Instance?.RegisterAttack(this, targetCombat, card.AttackDamage);
        }
    }

    public void SetTarget(Transform t)
    {
        if (t == null)
        {
            ClearTarget();
            return;
        }

        var newTarget = t.GetComponent<ICombatTarget>();
        if (newTarget != null && IsValidEnemy(newTarget))
        {
            targetTransform = t;
            targetCombat = newTarget;
        }
        else
        {
            ClearTarget();
        }
    }

    private void ClearTarget()
    {
        targetTransform = null;
        targetCombat = null;
    }

    private bool IsValidEnemy(ICombatTarget other)
    {
        if (other is KnightCombat knight) return knight.card.IsEnemy != card.IsEnemy;
        if (other is TowerCombat tower) return tower.IsEnemy != card.IsEnemy;
        return false;
    }

    // --- Public API ---
    public void Initialize(KnightCard newCard)
    {
        card = newCard;
        currentHealth = card.MaxHealth;
        UpdateHPUI();
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

        if (t != targetTransform || !IsValidEnemy(targetCombat))
            return false;

        float distance = CombatUtils.GetSurfaceDistance(transform, targetTransform);
        return distance <= card.AttackRange;
    }

    public override void TakeDamage(float amount)
    {
        if (IsDead) return;

        currentHealth -= amount;
        UpdateHPUI();

        if (currentHealth <= 0f)
            Die();
    }

    public override Transform GetTransform() => transform;

    // --- Death ---
    private void Die()
    {
        cooldownTimer = MaxCooldownValue;

        if (TryGetComponent(out UnityEngine.AI.NavMeshAgent agent))
            agent.isStopped = true;

        ClearTarget();
        Destroy(gameObject);
    }

    // --- UI ---
    private void UpdateHPUI()
    {
        if (hpText == null) return;

        string current = Mathf.Max(0, currentHealth).ToString("0");
        string max = card.MaxHealth.ToString("0");
        hpText.text = $"{current}\n―\n{max}";
    }

    private void UpdateUI()
    {
        if (hpText != null)
            hpText.color = wasInCombat ? combatColor : idleColor;
    }

    // --- Debug ---
    private void OnDrawGizmosSelected()
    {
        float range = card != null ? card.AttackRange : KnightCard.Template.AttackRange;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    private void OnDrawGizmos()
    {
        if (targetTransform == null) return;

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

#if UNITY_EDITOR
        float dist = Vector3.Distance(from, to);
        Vector3 mid = (from + to) * 0.5f;
        UnityEditor.Handles.Label(mid + Vector3.up * 0.5f, dist.ToString("F2"));
#endif
    }
}
