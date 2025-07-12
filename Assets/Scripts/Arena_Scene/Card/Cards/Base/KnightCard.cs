public class KnightCard
{
    public string CardName { get; set; }
    public int Cost { get; set; }
    public float MaxHealth { get; set; }
    public float AttackDamage { get; set; }
    public float AttackRange { get; set; }
    public float AttackCooldown { get; set; }
    public float MoveSpeed { get; set; }

    public static readonly KnightCard Template = new()
    {
        CardName = "Рыцарь",
        Cost = 3,
        MaxHealth = 150f,
        AttackDamage = 2f,
        AttackRange = 1.5f,
        AttackCooldown = 1.2f,
        MoveSpeed = 3.5f
    };

    public KnightCard Clone()
    {
        return (KnightCard)this.MemberwiseClone();
    }
}