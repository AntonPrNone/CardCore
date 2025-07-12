using UnityEngine;

// ����������� ������� ����� ��� ���� ������ (�������, ������� � �.�.)
public abstract class UnitBase : MonoBehaviour
{
    protected int maxHP = 100;            // ������������ ���������� ��������
    protected int damage = 10;            // ����, ������� ���� �������
    protected float attackInterval = 1.0f; // �������� ����� ������� (� ��������)
    protected float attackRange = 1.0f; // ��������� ��� ��������� �����
    protected int currentHP;           // ������� ��������
    protected float lastAttackTime;    // ����� ��������� �����

    // ������� ��� ������
    public int MaxHP => maxHP;
    public int Damage => damage;
    public float AttackInterval => attackInterval;
    public float AttackRange => attackRange;
    public int CurrentHP => currentHP; 

    // �������, ������� ���������� ��� ������ �����
    public event System.Action<UnitBase> OnDeath;

    // ������������� �������� �������� ��� ������
    protected virtual void Start()
    {
        currentHP = maxHP;
    }

    // ����� ��������� �����
    public virtual void TakeDamage(int amount, UnitBase target)
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
    public virtual void Attack(UnitBase target)
    {
        // �������� ������� ����� �������
        if (Time.time - lastAttackTime >= attackInterval)
        {
            lastAttackTime = Time.time;     // ��������� ����� ��������� �����
            target.TakeDamage(damage, target);     // ������� ���� ����
        }
    }

    // ����� ������ �����
    protected virtual void Die()
    {
        Debug.Log($"{name} ����.");
        OnDeath?.Invoke(this);             // �������� ������� ������
        Destroy(gameObject);              // ������� ������ �� �����
    }
}
