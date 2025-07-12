using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Боевая логика башни: атака, получение урона, управление здоровьем.
/// </summary>
[RequireComponent(typeof(Collider))]
public class TowerCombat : CombatEntity
{
    [Header("Stats")]
    [SerializeField] private float maxHealth = 200f;
    [SerializeField] private float attackDamage = 15f;
    [SerializeField] private float attackSpeed = 1f;
    [SerializeField] private float attackRange = 5f;

    [Header("Dissolve Settings")]
    [SerializeField] private float dissolveDuration = 1f;
    private static readonly int DissolveAmountID = Shader.PropertyToID("_DissolveAmount");

    [Header("Events")]
    [SerializeField] private UnityEvent onDeath;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private Color idleColor = Color.white;
    [SerializeField] private Color combatColor = Color.red;

    private float currentHealth;
    private ICombatTarget target;
    private bool inCombat;
    private Material dissolveMaterial;

    private const float MinHealth = 0f;

    public override bool IsDead => currentHealth <= MinHealth;

    void Start()
    {
        currentHealth = maxHealth;
        if (hpText == null) hpText = GetComponentInChildren<TextMeshProUGUI>();
        if (TryGetComponent(out Renderer renderer))
            dissolveMaterial = renderer.material;
        UpdateHPText();
    }

    void Update()
    {
        bool inCombatNow = target != null && !target.IsDead;
        if (!inCombat && inCombatNow)
            StartCoroutine(AttackRoutine());
        if (hpText != null)
            hpText.color = inCombatNow ? combatColor : idleColor;
    }

    public override void TakeDamage(float damage)
    {
        if (IsDead) return;
        currentHealth -= damage;
        UpdateHPText();
        if (currentHealth <= MinHealth)
            StartCoroutine(DissolveAndDie());
    }

    public void TakeDamage(float damage, Transform attacker)
    {
        if (attacker != null && attacker.TryGetComponent(out ICombatTarget combatTarget))
            SetTarget(combatTarget);
        TakeDamage(damage);
    }

    public void SetTarget(ICombatTarget newTarget)
    {
        target = newTarget;
    }

    private IEnumerator AttackRoutine()
    {
        inCombat = true;
        while (target != null && !IsDead && !target.IsDead)
        {
            float dist = CombatUtils.GetSurfaceDistance(transform, target.GetTransform());
            if (dist <= attackRange)
                CombatManager.Instance?.RegisterAttack(this, target, attackDamage);
            yield return new WaitForSeconds(1f / attackSpeed);
        }
        inCombat = false;
    }

    private IEnumerator DissolveAndDie()
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
            dissolveMaterial.SetFloat(DissolveAmountID, Mathf.Clamp01(elapsed / dissolveDuration));
            yield return null;
        }
        Destroy(gameObject);
    }

    private void UpdateHPText()
    {
        if (hpText != null)
        {
            string current = Mathf.Max(MinHealth, currentHealth).ToString("0");
            string max = maxHealth.ToString("0");
            hpText.text = $"{current}\n―\n{max}";
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}