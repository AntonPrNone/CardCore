using System;
using UnityEngine;

/// <summary>
/// Данные карты "Рыцарь" (Knight).
/// </summary>
[Serializable]
public class KnightCard
{
    [field: SerializeField] public string CardName { get; set; }
    [field: SerializeField] public int Cost { get; set; }
    [field: SerializeField] public float MaxHealth { get; set; }
    [field: SerializeField] public float AttackDamage { get; set; }
    [field: SerializeField] public float AttackRange { get; set; }
    [field: SerializeField] public float AttackCooldown { get; set; }
    [field: SerializeField] public float MoveSpeed { get; set; }
    [field: SerializeField] public bool IsEnemy { get; set; }

    public static readonly KnightCard Template = new KnightCard
    {
        CardName = "Рыцарь",
        Cost = 3,
        MaxHealth = 150f,
        AttackDamage = 2f,
        AttackRange = 1.5f,
        AttackCooldown = 1.2f,
        MoveSpeed = 3.5f,
        IsEnemy = false
    };

    public KnightCard Clone()
    {
        return new KnightCard
        {
            CardName = CardName,
            Cost = Cost,
            MaxHealth = MaxHealth,
            AttackDamage = AttackDamage,
            AttackRange = AttackRange,
            AttackCooldown = AttackCooldown,
            MoveSpeed = MoveSpeed,
            IsEnemy = IsEnemy
        };
    }
}