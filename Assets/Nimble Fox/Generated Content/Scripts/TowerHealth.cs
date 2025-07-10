using UnityEngine;

public class TowerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [Tooltip("Maximum health of this tower.")]
    public int maxHealth = 200;

    private int currentHealth;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    /// <summary>
    /// Apply damage to the tower. Destroys tower when health ≤ 0.
    /// </summary>
    /// <param name="amount">Damage amount.</param>
    public void ApplyDamage(int amount)
    {
        if (currentHealth <= 0 || amount <= 0)
            return;

        currentHealth -= amount;
        Debug.Log($"[TowerHealth] {gameObject.name} took {amount} damage. Current health: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        Debug.Log($"[TowerHealth] {gameObject.name} destroyed. Removing from scene.");
        Destroy(gameObject);
    }

    /// <summary>
    /// Expose current health.
    /// </summary>
    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    /// <summary>
    /// Expose max health.
    /// </summary>
    public int GetMaxHealth()
    {
        return maxHealth;
    }
}