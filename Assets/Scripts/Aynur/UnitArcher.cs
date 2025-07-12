using UnityEngine;

public class UnitArcher : UnitBase
{
    protected override void Start()
    {
        maxHP = 80;
        damage = 25;
        attackInterval = 2.0f;
        attackRange = 6.0f;
        base.Start();
    }
}
