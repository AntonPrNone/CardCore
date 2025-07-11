using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class KnightMovement : MonoBehaviour
{
    [Header("Targeting")]
    public string enemyTag = "Unit";
    public string enemyTowerTag = "Tower";
    public float targetCheckInterval = 0.5f;

    private NavMeshAgent agent;
    private Transform target;
    private float targetCheckTimer = 0f;
    private KnightCombat knightCombat;
    private float baseAttackRange;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        knightCombat = GetComponent<KnightCombat>();
        if (knightCombat != null)
            baseAttackRange = knightCombat.GetAttackRange();
        agent.stoppingDistance = 0f; // отключаем стандартную остановку
    }

    void Update()
    {
        targetCheckTimer -= Time.deltaTime;
        // Если есть цель и идёт бой — не ищем новую цель
        if (target != null && knightCombat != null && knightCombat.InCombatWithTarget(target))
        {
            // Только следим за дистанцией и остановкой
            HandleMovementToTarget();
            return;
        }

        // Если цели нет или цель мертва — ищем новую
        if (TargetIsDead(target) || targetCheckTimer <= 0f)
        {
            ChooseTarget();
            targetCheckTimer = targetCheckInterval;
        }

        HandleMovementToTarget();
    }

    void HandleMovementToTarget()
    {
        if (target != null)
        {
            float surfaceDistance = CombatUtils.GetSurfaceDistance(transform, target);
            if (surfaceDistance > baseAttackRange)
            {
                if (agent.isStopped) agent.isStopped = false;
                agent.SetDestination(target.position);
            }
            else
            {
                if (!agent.isStopped) agent.isStopped = true;
                agent.ResetPath();
                agent.velocity = Vector3.zero; // полностью гасим движение
            }
        }
        else if (agent.hasPath)
        {
            agent.ResetPath();
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }
    }

    bool TargetIsDead(Transform t)
    {
        if (t == null) return true;
        var combatTarget = t.GetComponent<ICombatTarget>();
        return combatTarget == null || combatTarget.IsDead;
    }

    void ChooseTarget()
    {
        Transform nearestEnemy = FindClosestWithTag(enemyTag);
        Transform nearestTower = FindClosestWithTag(enemyTowerTag);

        float distEnemy = nearestEnemy ? Vector3.Distance(transform.position, nearestEnemy.position) : Mathf.Infinity;
        float distTower = nearestTower ? Vector3.Distance(transform.position, nearestTower.position) : Mathf.Infinity;

        if (distEnemy == Mathf.Infinity && distTower == Mathf.Infinity)
        {
            if (target != null)
            {
                target = null;
                knightCombat?.SetTarget(null);
            }
            return;
        }

        Transform newTarget = distEnemy < distTower ? nearestEnemy : nearestTower;
        if (newTarget != target)
        {
            target = newTarget;
            knightCombat?.SetTarget(target);
        }
    }

    Transform FindClosestWithTag(string tag)
    {
        GameObject[] candidates = GameObject.FindGameObjectsWithTag(tag);
        Transform closest = null;
        float minDist = Mathf.Infinity;
        foreach (GameObject go in candidates)
        {
            if (go == gameObject) continue;
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