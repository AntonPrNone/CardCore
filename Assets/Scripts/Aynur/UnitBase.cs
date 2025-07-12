using UnityEngine;

// ����������� ������� ����� ��� ���� ������ (�������, ������� � �.�.)
public abstract class UnitBase : MonoBehaviour
{
    [Header("����� �����")]
    public int maxHP = 100;            // ������������ ���������� ��������
    public int damage = 10;            // ����, ������� ���� �������
    public float attackInterval = 1.0f; // �������� ����� ������� (� ��������)
    public float attackRange = 1.0f; // ��������� ��� ��������� �����

    protected int currentHP;           // ������� ��������
    public int CurrentHP => currentHP; // ������ ��� �������� ��������

    protected float lastAttackTime;    // ����� ��������� �����

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
