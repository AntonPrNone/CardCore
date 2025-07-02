using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class UnitMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    public string enemyTag = "Unit";
    public string enemyTowerTag = "Tower";

    public float targetCheckInterval = 0.5f;
    private float targetCheckTimer = 0f;

    private Transform target;
    private Vector2Int currentCell;
    private Vector2Int targetCell;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (!rb.isKinematic)
        {
            Debug.LogWarning($"{name}: Rigidbody должен быть isKinematic = true для корректного управления движением.");
            rb.isKinematic = true;
        }
    }

    void Update()
    {
        targetCheckTimer -= Time.deltaTime;
        if (target == null || targetCheckTimer <= 0f)
        {
            ChooseTarget();
            targetCheckTimer = targetCheckInterval;
        }

        if (target == null) return;

        MoveToTarget();
    }

    void ChooseTarget()
    {
        Transform nearestEnemy = FindClosestWithTag(enemyTag);
        Transform enemyTower = FindClosestWithTag(enemyTowerTag);

        float distToEnemy = nearestEnemy ? Vector3.Distance(transform.position, nearestEnemy.position) : Mathf.Infinity;
        float distToTower = enemyTower ? Vector3.Distance(transform.position, enemyTower.position) : Mathf.Infinity;

        target = distToEnemy < distToTower ? nearestEnemy : enemyTower;

        if (target != null)
        {
            targetCell = GridOnTerrainManager.Instance.GetGridCoordinates(target.position);

            // Устанавливаем цель в UnitCombat
            UnitCombat combat = GetComponent<UnitCombat>();
            if (combat != null)
                combat.SetTarget(target);
        }
    }

    Transform FindClosestWithTag(string tag)
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);
        Transform closest = null;
        float minDist = Mathf.Infinity;

        foreach (var go in targets)
        {
            if (go == this.gameObject) continue;

            float dist = Vector3.Distance(transform.position, go.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = go.transform;
            }
        }

        return closest;
    }

    void MoveToTarget()
    {
        currentCell = GridOnTerrainManager.Instance.GetGridCoordinates(transform.position);
        Vector2Int nextStep = GetStepTowardsTarget(currentCell, targetCell);

        if (nextStep == currentCell) return;

        Vector3 worldTarget = GridOnTerrainManager.Instance.GetWorldPosition(nextStep.x, nextStep.y);
        worldTarget.y = transform.position.y;

        Vector3 direction = (worldTarget - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, worldTarget);

        if (!Physics.Raycast(transform.position, direction, distance + 0.1f))
        {
            transform.position = Vector3.MoveTowards(transform.position, worldTarget, moveSpeed * Time.deltaTime);
        }
        else
        {
            Debug.Log($"{name}: путь заблокирован");
        }
    }

    Vector2Int GetStepTowardsTarget(Vector2Int from, Vector2Int to)
    {
        int dx = Mathf.Clamp(to.x - from.x, -1, 1);
        int dz = Mathf.Clamp(to.y - from.y, -1, 1);

        return new Vector2Int(from.x + dx, from.y + dz);
    }
}
