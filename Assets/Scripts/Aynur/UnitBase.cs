using UnityEngine;

// Абстрактный базовый класс для всех юнитов (мечники, стрелки и т.д.)
public abstract class UnitBase : MonoBehaviour
{
    protected int maxHP = 100;            // Максимальное количество здоровья
    protected int damage = 10;            // Урон, который юнит наносит
    protected float attackInterval = 1.0f; // Интервал между атаками (в секундах)
    protected float attackRange = 1.0f; // Дистанция для нанесения удара
    protected int currentHP;           // Текущее здоровье
    protected float lastAttackTime;    // Время последней атаки

    // геттеры для статов
    public int MaxHP => maxHP;
    public int Damage => damage;
    public float AttackInterval => attackInterval;
    public float AttackRange => attackRange;
    public int CurrentHP => currentHP; 

    // Событие, которое вызывается при смерти юнита
    public event System.Action<UnitBase> OnDeath;

    // Инициализация текущего здоровья при старте
    protected virtual void Start()
    {
        currentHP = maxHP;
    }

    // Метод получения урона
    public virtual void TakeDamage(int amount, UnitBase target)
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
    public virtual void Attack(UnitBase target)
    {
        // Проверка времени между ударами
        if (Time.time - lastAttackTime >= attackInterval)
        {
            lastAttackTime = Time.time;     // Обновляем время последней атаки
            target.TakeDamage(damage, target);     // Наносим урон цели
        }
    }

    // Метод смерти юнита
    protected virtual void Die()
    {
        Debug.Log($"{name} умер.");
        OnDeath?.Invoke(this);             // Вызываем событие смерти
        Destroy(gameObject);              // Удаляем объект из сцены
    }
}
