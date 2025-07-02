using UnityEngine;
using TMPro;
using System.Collections;

public class UnitCombat : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 100f;
    public float currentHealth;

    public float attackDamage = 10f;
    public float attackSpeed = 1f; // атак в секунду

    [Header("UI")]
    public TextMeshProUGUI hpText;

    private Transform target;
    private bool inCombat = false;

    void Start()
    {
        currentHealth = maxHealth;

        if (hpText == null)
            hpText = GetComponentInChildren<TextMeshProUGUI>();

        UpdateHPText();
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;

        if (target != null && !inCombat)
            StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        inCombat = true;

        while (target != null && !IsDead && !IsTargetDead())
        {
            // Наносим урон в зависимости от типа цели
            UnitCombat enemyUnit = target.GetComponent<UnitCombat>();
            if (enemyUnit != null)
            {
                enemyUnit.TakeDamage(attackDamage);
            }
            else
            {
                BuildingHealth building = target.GetComponent<BuildingHealth>();
                if (building != null)
                {
                    building.TakeDamage(attackDamage);
                }
            }

            yield return new WaitForSeconds(1f / attackSpeed);
        }

        inCombat = false;
    }

    bool IsTargetDead()
    {
        if (target == null) return true;

        UnitCombat enemyUnit = target.GetComponent<UnitCombat>();
        if (enemyUnit != null) return enemyUnit.IsDead;

        BuildingHealth building = target.GetComponent<BuildingHealth>();
        if (building != null) return building.IsDead;

        return true;
    }

    public bool IsDead => currentHealth <= 0;

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
        UpdateHPText();
    }

    void Die()
    {
        Destroy(gameObject);
    }

    void UpdateHPText()
    {
        if (hpText != null)
            hpText.text = Mathf.Max(0, Mathf.CeilToInt(currentHealth)).ToString();
    }
}
