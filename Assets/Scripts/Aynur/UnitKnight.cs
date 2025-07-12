using UnityEngine;

public class UnitKnight : UnitBase
{
    protected override void Start()
    {
        maxHP = 150;
        damage = 15;
        attackInterval = 1.2f;
        attackRange = 2.0f;
        base.Start();
    }
}