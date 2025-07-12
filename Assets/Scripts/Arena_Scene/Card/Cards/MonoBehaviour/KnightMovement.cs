using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class KnightMovement : MonoBehaviour
{
    [Header("Targeting")]
    [SerializeField] private string _enemyTag = "Unit";
    [SerializeField] private string _enemyTowerTag = "Tower";
    [SerializeField] private float _targetCheckInterval = 0.5f; // Увеличено до 1с для стабильности

    private NavMeshAgent _agent;
    private Transform _target;
    private float _targetCheckTimer = 0f;
    private KnightCombat _knightCombat;
    private float _baseAttackRange;


    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _knightCombat = GetComponent<KnightCombat>();
        if (_knightCombat != null)
            _baseAttackRange = _knightCombat.GetAttackRange();
        _agent.stoppingDistance = 0f;
    }

    void Update()
    {
        _targetCheckTimer -= Time.deltaTime;
        // Если есть цель и идёт бой — не ищем новую цель
        if (_target != null && _knightCombat != null && _knightCombat.InCombatWithTarget(_target))
        {
            HandleMovementToTarget();
            return;
        }
        // Если цели нет или цель мертва — ищем новую
        if (TargetIsDead(_target) || _targetCheckTimer <= 0f)
        {
            ChooseTarget();
            _targetCheckTimer = _targetCheckInterval;
        }
        HandleMovementToTarget();
    }

    private void HandleMovementToTarget()
    {
        if (_target != null)
        {
            bool isEnemyTarget = _target.GetComponent<ICombatTarget>() is KnightCombat knight ? knight.card.IsEnemy : (_target.GetComponent<ICombatTarget>() is TowerCombat tower ? tower.IsEnemy : false);
            bool isEnemySelf = _knightCombat != null && _knightCombat.card.IsEnemy;
            
            // Проверяем, что цель - враг (разные команды)
            if (isEnemyTarget == isEnemySelf)
            {
                _target = null;
                _knightCombat?.SetTarget(null);
            }
            else
            {
                float surfaceDistance = CombatUtils.GetSurfaceDistance(transform, _target);
                if (surfaceDistance > _baseAttackRange)
                {
                    if (_agent.isStopped) _agent.isStopped = false;
                    _agent.SetDestination(_target.position);
                }
                else
                {
                    if (!_agent.isStopped) _agent.isStopped = true;
                    _agent.ResetPath();
                    _agent.velocity = Vector3.zero;
                }
            }
        }
        else if (_agent.hasPath)
        {
            _agent.ResetPath();
            _agent.isStopped = true;
            _agent.velocity = Vector3.zero;
        }
    }

    private bool TargetIsDead(Transform t)
    {
        if (t == null) return true;
        var combatTarget = t.GetComponent<ICombatTarget>();
        return combatTarget == null || combatTarget.IsDead;
    }

    private void ChooseTarget()
    {
        Transform nearestEnemy = FindClosestWithTag(_enemyTag);
        Transform nearestTower = FindClosestWithTag(_enemyTowerTag);
        float distEnemy = nearestEnemy ? Vector3.Distance(transform.position, nearestEnemy.position) : Mathf.Infinity;
        float distTower = nearestTower ? Vector3.Distance(transform.position, nearestTower.position) : Mathf.Infinity;

        Transform newTarget = distEnemy < distTower ? nearestEnemy : nearestTower;
        if (newTarget != _target)
        {
            _target = newTarget;
            _knightCombat?.SetTarget(_target);
        }
    }

    private Transform FindClosestWithTag(string tag)
    {
        GameObject[] candidates = GameObject.FindGameObjectsWithTag(tag);
        Transform closest = null;
        float minDist = Mathf.Infinity;
        bool isEnemySelf = _knightCombat != null && _knightCombat.card.IsEnemy;

        foreach (GameObject go in candidates)
        {
            if (go == gameObject) continue;
            
            var combatTarget = go.GetComponent<ICombatTarget>();
            if (combatTarget == null) continue;
            
            bool isEnemyTarget = combatTarget is KnightCombat knight ? knight.card.IsEnemy : (combatTarget is TowerCombat tower ? tower.IsEnemy : false);
            
            // Ищем только врагов (разные команды)
            if (isEnemySelf == isEnemyTarget) continue;
            
            float d = Vector3.Distance(transform.position, go.transform.position);
            if (d < minDist)
            {
                minDist = d;
                closest = go.transform;
            }
        }

        return closest;
    }
}