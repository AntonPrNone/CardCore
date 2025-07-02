using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;

    private struct AttackRequest
    {
        public UnitCombat attacker;
        public UnitCombat target;
        public float damage;
    }

    private List<AttackRequest> attackQueue = new();
    private Dictionary<UnitCombat, float> damageAccumulator = new();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void RegisterAttack(UnitCombat attacker, UnitCombat target, float damage)
    {
        if (attacker == null || target == null) return;
        attackQueue.Add(new AttackRequest { attacker = attacker, target = target, damage = damage });
    }

    void FixedUpdate()
    {
        damageAccumulator.Clear();

        // Накопление урона
        foreach (var attack in attackQueue)
        {
            if (attack.target != null && !attack.target.IsDead)
            {
                if (!damageAccumulator.ContainsKey(attack.target))
                    damageAccumulator[attack.target] = 0f;
                damageAccumulator[attack.target] += attack.damage;
            }
        }
        attackQueue.Clear();

        // Применение урона одновременно
        foreach (var kvp in damageAccumulator)
        {
            kvp.Key.TakeDamage(kvp.Value);
        }
    }
}
