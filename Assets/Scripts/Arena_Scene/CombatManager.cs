using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;

    private struct AttackRequest
    {
        public ICombatTarget attacker;
        public ICombatTarget target;
        public float damage;
    }

    private readonly List<AttackRequest> attackQueue = new();
    private readonly Dictionary<ICombatTarget, float> damageAccumulator = new();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void RegisterAttack(ICombatTarget attacker, ICombatTarget target, float damage)
    {
        Debug.Log($"[CombatManager] Зарегистрирована атака: {attacker.GetTransform().name} → {target.GetTransform().name} на {damage}");
        if (attacker == null || target == null) return;

        attackQueue.Add(new AttackRequest { attacker = attacker, target = target, damage = damage });
    }


    void FixedUpdate()
    {
        foreach (var attack in attackQueue)
        {
            if (attack.target != null && !attack.target.IsDead)
            {
                // Если цель — башня, вызываем расширенный метод
                if (attack.target is TowerCombat tower)
                {
                    tower.TakeDamage(attack.damage, attack.attacker.GetTransform());
                }
                else
                {
                    // Обычный юнит
                    attack.target.TakeDamage(attack.damage);
                }
            }
        }

        attackQueue.Clear();
    }
}

public interface ICombatTarget
{
    bool IsDead { get; }
    void TakeDamage(float amount);
    Transform GetTransform();
}

public abstract class CombatEntity : MonoBehaviour, ICombatTarget
{
    public abstract bool IsDead { get; }
    public abstract void TakeDamage(float amount);
    public virtual Transform GetTransform() => transform;
}
