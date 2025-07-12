using UnityEngine;

[CreateAssetMenu(fileName = "UnitClassData", menuName = "Scriptable Objects/UnitClassData")]
public class UnitClassData : ScriptableObject
{
    public string className;
    public int maxHP;
    public int damage;
    public float attackRange;
    public float attackInterval;
}
