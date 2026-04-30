using System;
using UnityEngine;
using UnityEngine.AI;

public abstract class AIController : MonoBehaviour
{
    protected NavMeshAgent agent;

    public event Action OnReachedDestination;
    public event Action OnDestinationChanged;
    public event Action OnMovementFailed;

    protected Vector3? currentTarget;

    public bool HasReachedDestination
    {
        get
        {
            if (agent.pathPending) return false;
            return agent.remainingDistance <= agent.stoppingDistance;
        }
    }

    public bool IsMoving => !agent.isStopped && agent.velocity.sqrMagnitude > 0.01f;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    protected virtual void Update()
    {
        if (currentTarget.HasValue && HasReachedDestination)
        {
            agent.isStopped = true;
            currentTarget = null;
            OnReachedDestination?.Invoke();
        }
    }

    public bool MoveTo(Vector3 destination)
    {
        NavMeshHit hit;
        if (!NavMesh.SamplePosition(destination, out hit, 1f, NavMesh.AllAreas))
        {
            OnMovementFailed?.Invoke();
            return false;
        }

        agent.isStopped = false;
        agent.SetDestination(hit.position);
        currentTarget = hit.position;
        OnDestinationChanged?.Invoke();
        return true;
    }

    public bool MoveToRandomNavMeshPoint(float radius)
    {
        Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * radius;
        Vector3 randomPoint = transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);

        NavMeshHit hit;
        if (!NavMesh.SamplePosition(randomPoint, out hit, radius, NavMesh.AllAreas))
            return false;

        return MoveTo(hit.position);
    }

    public void TeleportTo(Vector3 position)
    {
        agent.Warp(position);
    }

    public void StopMovement()
    {
        agent.isStopped = true;
        agent.ResetPath();
        currentTarget = null;
    }

    public void ResumeMovement()
    {
        if (!currentTarget.HasValue) return;
        agent.isStopped = false;
    }

    public void PauseMovement()
    {
        agent.isStopped = true;
    }

    public void EnableAvoidance(ObstacleAvoidanceType quality, int priority = 50)
    {
        agent.obstacleAvoidanceType = quality;
        agent.avoidancePriority = Mathf.Clamp(priority, 0, 99);
    }

    public void DisableAvoidance()
    {
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
    }
}