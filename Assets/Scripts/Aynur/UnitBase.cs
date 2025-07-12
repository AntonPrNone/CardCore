using UnityEngine;

// ����������� ������� ����� ��� ���� ������ (�������, ������� � �.�.)
public class UnitBase : MonoBehaviour
{
    protected int maxHP;                // ������������ ���������� ��������
    protected int damage;               // ����, ������� ���� �������
    protected float attackInterval;     // �������� ����� ������� (� ��������)
    protected float attackRange;        // ��������� ��� ��������� �����

    protected int currentHP;            // ������� ��������
    protected float lastAttackTime;     // ����� ��������� �����

    public UnitClassData classData;

    // ������� ��� ������
    public int MaxHP => maxHP;
    public int Damage => damage;
    public float AttackInterval => attackInterval;
    public float AttackRange => attackRange;
    public int CurrentHP => currentHP; 

    // �������, ������� ���������� ��� ������ �����
    public event System.Action<UnitBase> OnDeath;

    // ������������� �������� �������� ��� ������
    protected void Start()
    {
        // ��������� ��������� �� ScriptableObject
        maxHP = classData.maxHP;
        damage = classData.damage;
        attackRange = classData.attackRange;
        attackInterval = classData.attackInterval;

        currentHP = maxHP;
    }

    // ����� ��������� �����
    public void TakeDamage(int amount, UnitBase target)
    {
        currentHP -= amount; // ��������� ��������
        Debug.Log($"{name} ������� ����: {amount} �� {target.name}, �������� HP: {currentHP}");

        // ���� �������� ����� �� ���� ��� ���� � �������
        if (currentHP <= 0)
        {
            Die();
        }
    }

    // ����� ����� �� ����
    public void Attack(UnitBase target)
    {
        // �������� ������� ����� �������
        if (Time.time - lastAttackTime >= attackInterval)
        {
            lastAttackTime = Time.time;     // ��������� ����� ��������� �����
            target.TakeDamage(damage, target);     // ������� ���� ����
        }
    }

    // ����� ������ �����
    protected void Die()
    {
        Debug.Log($"{name} ����.");
        OnDeath?.Invoke(this);             // �������� ������� ������
        Destroy(gameObject);              // ������� ������ �� �����
    }
}
