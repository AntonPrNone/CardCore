using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(FollowerEntity))]
public class KnightMovement : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private string _enemyTag = "Unit";
    [SerializeField] private string _enemyTowerTag = "Tower";
    [SerializeField] private float _checkTargetInterval = 1f;

    private FollowerEntity _agent;
    private KnightCombat _combat;
    private Transform _currentTarget;
    private float _lastTargetCheck;

    void Start()
    {
        _agent = GetComponent<FollowerEntity>();
        _combat = GetComponent<KnightCombat>();
        
        // Базовая настройка
        _agent.maxSpeed = _combat?.card.MoveSpeed ?? 3.5f;
        
        // Автоматический пересчет пути
        var autoRepath = _agent.autoRepath;
        autoRepath.mode = AutoRepathPolicy.Mode.EveryNSeconds;
        autoRepath.period = _checkTargetInterval;
        _agent.autoRepath = autoRepath;
    }

    void Update()
    {
        // Проверяем цель каждые N секунд
        if (Time.time - _lastTargetCheck > _checkTargetInterval)
        {
            FindNewTarget();
            _lastTargetCheck = Time.time;
        }

        // Если есть цель - двигаемся к ней
        if (_currentTarget != null)
        {
            MoveToTarget();
        }
    }

    private void FindNewTarget()
    {
        // Ищем ближайшего врага
        Transform nearestEnemy = FindNearestEnemy(_enemyTag);
        Transform nearestTower = FindNearestEnemy(_enemyTowerTag);

        // Выбираем ближайшую цель
        Transform newTarget = null;
        float enemyDist = nearestEnemy ? Vector3.Distance(transform.position, nearestEnemy.position) : float.MaxValue;
        float towerDist = nearestTower ? Vector3.Distance(transform.position, nearestTower.position) : float.MaxValue;

        if (enemyDist < towerDist)
            newTarget = nearestEnemy;
        else
            newTarget = nearestTower;

        // Обновляем цель
        if (newTarget != _currentTarget)
        {
            _currentTarget = newTarget;
            _combat?.SetTarget(newTarget);
            
            // Пересчитываем stopDistance только при смене цели
            if (newTarget != null)
            {
                float baseAttackRange = _combat?.GetAttackRange() ?? 2f;
                float compensation = GetColliderCompensation();
                _agent.stopDistance = baseAttackRange + compensation;
            }
        }
    }

    private Transform FindNearestEnemy(string tag)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
        Transform nearest = null;
        float minDistance = float.MaxValue;
        bool isEnemySelf = _combat?.card.IsEnemy ?? false;

        foreach (GameObject obj in objects)
        {
            if (obj == gameObject) continue;

            var combatTarget = obj.GetComponent<ICombatTarget>();
            if (combatTarget == null) continue;

            // Проверяем, что это враг
            bool isEnemyTarget = false;
            if (combatTarget is KnightCombat knight)
                isEnemyTarget = knight.card.IsEnemy;
            else if (combatTarget is TowerCombat tower)
                isEnemyTarget = tower.IsEnemy;

            if (isEnemyTarget == isEnemySelf) continue; // Из той же команды

            float distance = Vector3.Distance(transform.position, obj.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = obj.transform;
            }
        }

        return nearest;
    }

    private void MoveToTarget()
    {
        if (_currentTarget == null) return;

        // Устанавливаем цель - A* сам остановится на stopDistance
        _agent.destination = _currentTarget.position;
    }

    private float GetColliderCompensation()
    {
        // Получаем размеры коллайдеров персонажа и цели
        Collider myCollider = GetComponent<Collider>();
        if (_currentTarget != null)
        {
            Collider targetCollider = _currentTarget.GetComponent<Collider>();
            if (myCollider != null && targetCollider != null)
            {
                // Примерная компенсация на основе размеров коллайдеров
                float myRadius = CombatUtils.GetColliderApproxRadius(myCollider);
                float targetRadius = CombatUtils.GetColliderApproxRadius(targetCollider);
                return myRadius + targetRadius;
            }
        }
        return 0f;
    }
}