using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class UnitMovement : MonoBehaviour
{
    [Header("Targeting")]
    public string enemyTag = "Unit";
    public string enemyTowerTag = "Tower";
    public float targetCheckInterval = 0.5f;

    private NavMeshAgent agent;
    private Transform target;
    private float targetCheckTimer;
    private UnitCombat unitCombat;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        unitCombat = GetComponent<UnitCombat>();
        if (agent == null)
        {
            Debug.LogError($"{name}: NavMeshAgent component is required.");
        }
    }

    void Start()
    {
        targetCheckTimer = 0f;
    }

    void Update()
    {
        targetCheckTimer -= Time.deltaTime;

        bool targetIsDead = TargetIsDead(target);

        if (targetIsDead || targetCheckTimer <= 0f)
        {
            ChooseTarget();
            targetCheckTimer = targetCheckInterval;
        }

        if (target != null && !target.Equals(null))
        {
            agent.SetDestination(target.position);
        }
        else
        {
            // Нет цели — сбрасываем путь, чтобы юнит не ходил "куда-то"
            if (agent.hasPath)
            {
                agent.ResetPath();
            }
        }
    }

    bool TargetIsDead(Transform t)
    {
        if (t == null || t.Equals(null))
            return true;

        var combat = t.GetComponent<UnitCombat>();
        return combat == null || combat.IsDead;
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
                if (unitCombat != null)
                    unitCombat.SetTarget(null);
            }
            return;
        }

        Transform newTarget = distEnemy < distTower ? nearestEnemy : nearestTower;

        if (newTarget != target)
        {
            target = newTarget;
            if (unitCombat != null)
                unitCombat.SetTarget(target);
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
