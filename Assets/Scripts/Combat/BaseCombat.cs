using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public abstract class BaseCombat : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 100f;
    public float attackDamage = 10f;
    public float attackSpeed = 1f;

    [Header("Dissolve Settings")]
    public float dissolveDuration = 1f;
    protected static readonly int DissolveAmountID = Shader.PropertyToID("_DissolveAmount");

    [Header("Events")]
    public UnityEvent onDeath;

    [Header("UI")]
    public TextMeshProUGUI hpText;

    public float currentHealth;
    protected Transform target;
    protected bool inCombat;
    protected Material dissolveMaterial;

    public bool IsDead => currentHealth <= 0;

    protected virtual void Start()
    {
        currentHealth = maxHealth;

        if (hpText == null)
            hpText = GetComponentInChildren<TextMeshProUGUI>();

        if (TryGetComponent(out Renderer renderer))
            dissolveMaterial = renderer.material;

        UpdateHPText();
    }

    protected virtual void Update()
    {
        if (target != null && !inCombat && !IsTargetDead())
            StartCoroutine(AttackRoutine());
    }

    public virtual void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    IEnumerator AttackRoutine()
    {
        inCombat = true;

        while (target != null && !IsDead && !IsTargetDead())
        {
            BaseCombat targetCombat = target.GetComponent<BaseCombat>();
            if (targetCombat != null)
                targetCombat.TakeDamage(attackDamage);

            yield return new WaitForSeconds(1f / attackSpeed);
        }

        inCombat = false;
    }

    public virtual void TakeDamage(float damage)
    {
        if (IsDead) return;

        currentHealth -= damage;
        UpdateHPText();

        if (currentHealth <= 0)
            StartCoroutine(DissolveAndDie());
    }

    protected virtual IEnumerator DissolveAndDie()
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

    protected virtual bool IsTargetDead()
    {
        if (target == null) return true;

        BaseCombat combat = target.GetComponent<BaseCombat>();
        return combat == null || combat.IsDead;
    }

    protected virtual void UpdateHPText()
    {
        if (hpText != null)
            hpText.text = Mathf.Max(0, Mathf.CeilToInt(currentHealth)).ToString();
    }
}
