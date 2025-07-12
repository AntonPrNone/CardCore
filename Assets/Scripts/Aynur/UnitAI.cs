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
    [Header("AI Параметры")]
    public float orbitRadius = 0.5f;    // Отступ от цели (чтобы не толпились)

    private TeamType teamType;          // Тип команды (определяется по тегу)

    private string[] blueTargetTags = { "RedUnit", "RedTower" };   // Теги целей для синих
    private string[] redTargetTags = { "BlueUnit", "BlueTower" };  // Теги целей для красных

    private NavMeshAgent agent;               // Ссылка на NavMeshAgent
    private UnitBase selfUnit;                // Ссылка на свой UnitBase
    private UnitBase currentTargetUnit;       // Ссылка на цель как UnitBase
    private Transform currentTargetTransform; // Цель как Transform (для навигации)
    private Vector3 finalTargetPoint;         // Точка, куда агент идет

    TextMeshProUGUI healthText;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        selfUnit = GetComponent<UnitBase>();
        healthText = GetComponentInChildren<TextMeshProUGUI>();

        healthText.text = selfUnit.CurrentHP.ToString();

        // Определяем команду по тегу объекта
        if (CompareTag("RedUnit"))
            teamType = TeamType.Red;
        else if (CompareTag("BlueUnit"))
            teamType = TeamType.Blue;
        else
            Debug.LogWarning($"Неизвестный тег: {tag} у объекта {name}");

        agent.stoppingDistance = selfUnit.AttackRange - 1f;
        agent.autoBraking = true;

        FindNewTarget();
    }

    void Update()
    {
        // Если цели нет или она мертва/деактивирована — ищем новую
        if (currentTargetUnit == null || !currentTargetTransform.gameObject.activeInHierarchy)
        {
            FindNewTarget();
            return;
        }

        // Расстояние до цели
        float dist = Vector3.Distance(transform.position, currentTargetTransform.position);

        if (dist <= selfUnit.AttackRange)
        {
            // Если в радиусе атаки — атакуем
            selfUnit.Attack(currentTargetUnit);
            agent.isStopped = true;
        }
        else
        {
            // Если далеко — продолжаем двигаться
            UpdatePath();
        }

        healthText.text = selfUnit.CurrentHP.ToString();
    }

    // Поиск ближайшей цели с нужным тегом
    void FindNewTarget()
    {
        Transform closest = null;
        float minDist = Mathf.Infinity;
        Vector3 myPos = transform.position;
        UnitBase foundUnit = null;

        // Выбираем массив тегов в зависимости от команды
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

        // Назначаем цель
        currentTargetTransform = closest;
        currentTargetUnit = foundUnit;

        // Подписываемся на смерть цели
        if (currentTargetUnit != null)
            currentTargetUnit.OnDeath += OnTargetDeath;

        // Стартовое назначение маршрута
        UpdatePath();
    }

    void OnTargetDeath(UnitBase dead)
    {
        // Если это была наша цель — сбрасываем
        if (dead == currentTargetUnit)
        {
            currentTargetUnit.OnDeath -= OnTargetDeath;
            currentTargetUnit = null;
            currentTargetTransform = null;
        }
    }

    void OnDisable()
    {
        // Отписка от событий
        if (currentTargetUnit != null)
            currentTargetUnit.OnDeath -= OnTargetDeath;
    }

    // Обновление маршрута к цели
    void UpdatePath()
    {
        if (currentTargetTransform == null) return;

        // Вектор направления от цели к нам
        Vector3 dir = (transform.position - currentTargetTransform.position).normalized;

        // Смещаемся от центра цели на orbitRadius
        finalTargetPoint = currentTargetTransform.position + dir * orbitRadius;

        agent.SetDestination(finalTargetPoint);
        agent.isStopped = false;
    }
}
