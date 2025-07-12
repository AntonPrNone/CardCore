using UnityEngine;

// Абстрактный базовый класс для всех юнитов (мечники, стрелки и т.д.)
public class UnitBase : MonoBehaviour
{
    protected int maxHP;                // Максимальное количество здоровья
    protected int damage;               // Урон, который юнит наносит
    protected float attackInterval;     // Интервал между атаками (в секундах)
    protected float attackRange;        // Дистанция для нанесения удара

    protected int currentHP;            // Текущее здоровье
    protected float lastAttackTime;     // Время последней атаки

    public UnitClassData classData;

    // геттеры для статов
    public int MaxHP => maxHP;
    public int Damage => damage;
    public float AttackInterval => attackInterval;
    public float AttackRange => attackRange;
    public int CurrentHP => currentHP; 

    // Событие, которое вызывается при смерти юнита
    public event System.Action<UnitBase> OnDeath;

    // Инициализация текущего здоровья при старте
    protected void Start()
    {
        // Применяем параметры из ScriptableObject
        maxHP = classData.maxHP;
        damage = classData.damage;
        attackRange = classData.attackRange;
        attackInterval = classData.attackInterval;

        currentHP = maxHP;
    }

    // Метод получения урона
    public void TakeDamage(int amount, UnitBase target)
    {
        currentHP -= amount; // Уменьшаем здоровье
        Debug.Log($"{name} получил урон: {amount} от {target.name}, осталось HP: {currentHP}");

        // Если здоровье упало до нуля или ниже — умираем
        if (currentHP <= 0)
        {
            Die();
        }
    }

    // Метод атаки по цели
    public void Attack(UnitBase target)
    {
        // Проверка времени между ударами
        if (Time.time - lastAttackTime >= attackInterval)
        {
            lastAttackTime = Time.time;     // Обновляем время последней атаки
            target.TakeDamage(damage, target);     // Наносим урон цели
        }
    }

    // Метод смерти юнита
    protected void Die()
    {
        Debug.Log($"{name} умер.");
        OnDeath?.Invoke(this);             // Вызываем событие смерти
        Destroy(gameObject);              // Удаляем объект из сцены
    }
}
