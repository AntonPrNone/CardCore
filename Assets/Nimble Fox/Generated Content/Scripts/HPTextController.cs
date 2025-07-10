using UnityEngine;

[RequireComponent(typeof(TextMesh))]
public class HPTextController : MonoBehaviour
{
    // Controls the health text UI element.
    // Changes the health text color to red when the unit enters combat.
    // Reverts the health text color to the original color when the unit exits combat.
    // Updates the health text display to show:
    // - Current health on the first line
    // - A horizontal line on the second line
    // - Total health on the third line
    // This script exposes public methods for other systems to call.

    private TextMesh textMesh;
    private Color originalColor;
    private int currentHealth;
    private int totalHealth;

    private void Awake()
    {
        textMesh = GetComponent<TextMesh>();
        if (textMesh == null)
        {
            Debug.LogError("HPTextController requires a TextMesh component on the same GameObject.");
        }
        else
        {
            originalColor = textMesh.color;
        }
    }

    /// <summary>
    /// Call this when the unit's health changes.
    /// </summary>
    /// <param name="current">Current health value</param>
    /// <param name="total">Total/max health value</param>
    public void OnHealthChanged(int current, int total)
    {
        currentHealth = current;
        totalHealth = total;
        UpdateText();
    }

    /// <summary>
    /// Call this when the unit enters combat.
    /// </summary>
    public void OnEnterCombat()
    {
        if (textMesh != null)
            textMesh.color = Color.red;
    }

    /// <summary>
    /// Call this when the unit exits combat.
    /// </summary>
    public void OnExitCombat()
    {
        if (textMesh != null)
            textMesh.color = originalColor;
    }

    private void UpdateText()
    {
        if (textMesh == null) return;

        // Horizontal line - adjust the number of dashes if needed for your font/size.
        const string line = "-----";
        textMesh.text = $"{currentHealth}\n{line}\n{totalHealth}";
    }
}