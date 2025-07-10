using System.Collections;
using UnityEngine;

public class UnitHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [Tooltip("Maximum health of this unit.")]
    public int maxHealth = 100;

    [Tooltip("Time in seconds after last damage to consider combat ended.")]
    public float combatCooldown = 5f;

    private int currentHealth;
    private bool inCombat = false;
    private Coroutine combatExitCoroutine;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    /// <summary>
    /// Apply damage to this unit. Triggers combat state and destroys when health ≤ 0.
    /// </summary>
    /// <param name="amount">Amount of damage to apply.</param>
    public void ApplyDamage(int amount)
    {
        if (currentHealth <= 0 || amount <= 0)
            return;

        currentHealth -= amount;
        Debug.Log($"[UnitHealth] {gameObject.name} took {amount} damage. Current health: {currentHealth}/{maxHealth}");

        if (!inCombat)
            EnterCombat();

        // Restart exit-combat timer
        if (combatExitCoroutine != null)
            StopCoroutine(combatExitCoroutine);
        combatExitCoroutine = StartCoroutine(CombatExitTimer());

        if (currentHealth <= 0)
            Die();
    }

    private void EnterCombat()
    {
        inCombat = true;
        Debug.Log($"[UnitHealth] {gameObject.name} entered combat.");
        // If you have a BattleManager or UI listener, you could invoke an event here.
    }

    private IEnumerator CombatExitTimer()
    {
        yield return new WaitForSeconds(combatCooldown);
        inCombat = false;
        Debug.Log($"[UnitHealth] {gameObject.name} exited combat.");
        // Notify BattleManager/UI if needed.
    }

    private void Die()
    {
        Debug.Log($"[UnitHealth] {gameObject.name} died. Destroying GameObject.");
        Destroy(gameObject);
    }

    /// <summary>
    /// Optional: Heal the unit by a given amount, clamps to maxHealth.
    /// </summary>
    public void Heal(int amount)
    {
        if (amount <= 0 || currentHealth <= 0)
            return;
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log($"[UnitHealth] {gameObject.name} healed {amount}. Current health: {currentHealth}/{maxHealth}");
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