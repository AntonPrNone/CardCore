using UnityEngine;
using TMPro;

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
    public Color idleColor = Color.white;
    public Color combatColor = Color.red;

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

    void PerformAttack()
    {
        if (targetCombat != null && !targetCombat.IsDead)
            CombatManager.Instance?.RegisterAttack(this, targetCombat, card.AttackDamage);
    }

    public float GetAttackRange()
    {
        card ??= KnightCard.Template.Clone();
        return card.AttackRange;
    }

    public override void TakeDamage(float amount)
    {
        if (IsDead) return;

        currentHealth -= amount;
        UpdateHPUI();

        if (currentHealth <= 0f)
            Die();
    }

    void UpdateHPUI()
    {
        if (hpText != null)
        {
            string current = Mathf.Max(0, currentHealth).ToString("0");
            string max = card.MaxHealth.ToString("0");
            hpText.text = $"{current}\n―\n{max}";
        }
    }

    void Die()
    {
        cooldownTimer = float.MaxValue;

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
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, targetTransform.position);
        }
    }
}