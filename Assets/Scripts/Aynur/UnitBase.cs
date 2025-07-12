using UnityEngine;

// Абстрактный базовый класс для всех юнитов (мечники, стрелки и т.д.)
public abstract class UnitBase : MonoBehaviour
{
    [Header("Статы юнита")]
    public int maxHP = 100;            // Максимальное количество здоровья
    public int damage = 10;            // Урон, который юнит наносит
    public float attackInterval = 1.0f; // Интервал между атаками (в секундах)
    public float attackRange = 1.0f; // Дистанция для нанесения удара

    protected int currentHP;           // Текущее здоровье
    public int CurrentHP => currentHP; // геттер для текущего здоровья

    protected float lastAttackTime;    // Время последней атаки

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
