using TMPro;
using UnityEngine;
using UnityEngine.AI;

enum TeamType
{
    Blue,
    Red
}

[RequireComponent(typeof(NavMeshAgent), typeof(UnitBase))]
public class UnitAI : MonoBehaviour
{
    [Header("AI ���������")]
    public float orbitRadius = 0.5f;    // ������ �� ���� (����� �� ���������)

    private TeamType teamType;          // ��� ������� (������������ �� ����)

    private string[] blueTargetTags = { "RedUnit", "RedTower" };   // ���� ����� ��� �����
    private string[] redTargetTags = { "BlueUnit", "BlueTower" };  // ���� ����� ��� �������

    private NavMeshAgent agent;               // ������ �� NavMeshAgent
    private UnitBase selfUnit;                // ������ �� ���� UnitBase
    private UnitBase currentTargetUnit;       // ������ �� ���� ��� UnitBase
    private Transform currentTargetTransform; // ���� ��� Transform (��� ���������)
    private Vector3 finalTargetPoint;         // �����, ���� ����� ����

    TextMeshProUGUI healthText;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        selfUnit = GetComponent<UnitBase>();
        healthText = GetComponentInChildren<TextMeshProUGUI>();

        healthText.text = selfUnit.CurrentHP.ToString();

        // ���������� ������� �� ���� �������
        if (CompareTag("RedUnit"))
            teamType = TeamType.Red;
        else if (CompareTag("BlueUnit"))
            teamType = TeamType.Blue;
        else
            Debug.LogWarning($"����������� ���: {tag} � ������� {name}");

        agent.stoppingDistance = selfUnit.AttackRange - 1f;
        agent.autoBraking = true;

        FindNewTarget();
    }

    void Update()
    {
        // ���� ���� ��� ��� ��� ������/�������������� � ���� �����
        if (currentTargetUnit == null || !currentTargetTransform.gameObject.activeInHierarchy)
        {
            FindNewTarget();
            return;
        }

        // ���������� �� ����
        float dist = Vector3.Distance(transform.position, currentTargetTransform.position);

        if (dist <= selfUnit.AttackRange)
        {
            // ���� � ������� ����� � �������
            selfUnit.Attack(currentTargetUnit);
            agent.isStopped = true;
        }
        else
        {
            // ���� ������ � ���������� ���������
            UpdatePath();
        }

        healthText.text = selfUnit.CurrentHP.ToString();
    }

    // ����� ��������� ���� � ������ �����
    void FindNewTarget()
    {
        Transform closest = null;
        float minDist = Mathf.Infinity;
        Vector3 myPos = transform.position;
        UnitBase foundUnit = null;

        // �������� ������ ����� � ����������� �� �������
        string[] targetTags = (teamType == TeamType.Blue) ? blueTargetTags : redTargetTags;

        foreach (string tag in targetTags)
        {
            GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);

            foreach (GameObject obj in targets)
            {
                if (obj == gameObject) continue;

                float dist = Vector3.Distance(myPos, obj.transform.position);
                if (dist < minDist)
                {
                    UnitBase ub = obj.GetComponent<UnitBase>();
                    if (ub != null)
                    {
                        minDist = dist;
                        closest = obj.transform;
                        foundUnit = ub;
                    }
                }
            }
        }

        // ��������� ����
        currentTargetTransform = closest;
        currentTargetUnit = foundUnit;

        // ������������� �� ������ ����
        if (currentTargetUnit != null)
            currentTargetUnit.OnDeath += OnTargetDeath;

        // ��������� ���������� ��������
        UpdatePath();
    }

    void OnTargetDeath(UnitBase dead)
    {
        // ���� ��� ���� ���� ���� � ����������
        if (dead == currentTargetUnit)
        {
            currentTargetUnit.OnDeath -= OnTargetDeath;
            currentTargetUnit = null;
            currentTargetTransform = null;
        }
    }

    void OnDisable()
    {
        // ������� �� �������
        if (currentTargetUnit != null)
            currentTargetUnit.OnDeath -= OnTargetDeath;
    }

    // ���������� �������� � ����
    void UpdatePath()
    {
        if (currentTargetTransform == null) return;

        // ������ ����������� �� ���� � ���
        Vector3 dir = (transform.position - currentTargetTransform.position).normalized;

        // ��������� �� ������ ���� �� orbitRadius
        finalTargetPoint = currentTargetTransform.position + dir * orbitRadius;

        agent.SetDestination(finalTargetPoint);
        agent.isStopped = false;
    }
}
