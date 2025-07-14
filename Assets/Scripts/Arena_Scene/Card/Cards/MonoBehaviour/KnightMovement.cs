using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(FollowerEntity))]
public class KnightMovement : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private string _enemyTag = "Unit";
    [SerializeField] private string _enemyTowerTag = "Tower";
    [SerializeField] private float _checkTargetInterval = 0.5f;

    private FollowerEntity _agent;
    private KnightCombat _combat;
    private Transform _currentTarget;
    private float _lastTargetCheck;

    void Start()
    {
        _agent = GetComponent<FollowerEntity>();
        _combat = GetComponent<KnightCombat>();

        SetupAgent();
    }

    void Update()
    {
        if (!IsTargetValid() && Time.time - _lastTargetCheck > _checkTargetInterval)
        {
            UpdateTarget();
            _lastTargetCheck = Time.time;
        }

        if (_currentTarget != null)
        {
            _agent.destination = _currentTarget.position;
        }
    }

    private void SetupAgent()
    {
        _agent.maxSpeed = _combat?.card.MoveSpeed ?? 3.5f;

        var autoRepath = _agent.autoRepath;
        autoRepath.mode = AutoRepathPolicy.Mode.EveryNSeconds;
        autoRepath.period = _checkTargetInterval;
        _agent.autoRepath = autoRepath;
    }

    private bool IsTargetValid()
    {
        var combatTarget = _currentTarget?.GetComponent<ICombatTarget>();
        return combatTarget != null && !combatTarget.IsDead;
    }

    private void UpdateTarget()
    {
        Transform newTarget = GetNearestValidTarget();
        if (newTarget == _currentTarget) return;

        _currentTarget = newTarget;
        _combat?.SetTarget(newTarget);
        _agent.stopDistance = (_combat?.GetAttackRange() ?? 2f) + GetColliderCompensation(newTarget);
    }

    private Transform GetNearestValidTarget()
    {
        Transform enemy = FindNearestEnemy(_enemyTag);
        Transform tower = FindNearestEnemy(_enemyTowerTag);

        float distEnemy = enemy ? Vector3.Distance(transform.position, enemy.position) : float.MaxValue;
        float distTower = tower ? Vector3.Distance(transform.position, tower.position) : float.MaxValue;

        return distEnemy < distTower ? enemy : tower;
    }

    private Transform FindNearestEnemy(string tag)
    {
        GameObject[] candidates = GameObject.FindGameObjectsWithTag(tag);
        Transform nearest = null;
        float minDistance = float.MaxValue;

        bool isEnemySelf = _combat?.card.IsEnemy ?? false;

        foreach (var obj in candidates)
        {
            if (obj == gameObject) continue;

            var target = obj.GetComponent<ICombatTarget>();
            if (target == null || IsSameTeam(target, isEnemySelf)) continue;

            float distance = Vector3.Distance(transform.position, obj.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = obj.transform;
            }
        }

        return nearest;
    }

    private bool IsSameTeam(ICombatTarget target, bool selfIsEnemy)
    {
        return target switch
        {
            KnightCombat knight => knight.card.IsEnemy == selfIsEnemy,
            TowerCombat tower => tower.IsEnemy == selfIsEnemy,
            _ => true
        };
    }

    private float GetColliderCompensation(Transform target)
    {
        Collider myCollider = GetComponent<Collider>();
        Collider targetCollider = target?.GetComponent<Collider>();

        if (myCollider != null && targetCollider != null)
        {
            float myRadius = CombatUtils.GetColliderApproxRadius(myCollider);
            float targetRadius = CombatUtils.GetColliderApproxRadius(targetCollider);
            return myRadius + targetRadius;
        }

        return 0f;
    }
}
