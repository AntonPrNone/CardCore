using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Represents the spawned character on the map.
/// Ensures necessary movement and combat components are present.
/// </summary>
[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class SpawnedCharacter : MonoBehaviour
{
    void Awake()
    {
        // Ensure NavMeshAgent is present
        if (!TryGetComponent<NavMeshAgent>(out _))
            gameObject.AddComponent<NavMeshAgent>();

        // Ensure KnightMovement and KnightCombat are present
        if (!TryGetComponent<KnightMovement>(out _))
            gameObject.AddComponent<KnightMovement>();
        if (!TryGetComponent<KnightCombat>(out _))
            gameObject.AddComponent<KnightCombat>();

        // Optionally configure Rigidbody for NavMeshAgent
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }
}