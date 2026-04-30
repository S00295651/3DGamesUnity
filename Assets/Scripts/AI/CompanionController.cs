using UnityEngine;

public class CompanionController : AIController
{
    public Transform playerTransform;
    public float resumeFollowDistance = 3f;

    private bool isFollowing = false;
    private Vector3 lastKnownPlayerPosition;

    public float baseSpeed = 3.5f;
    public float maxSpeed = 30f;
    public float growthRate = 0.01f;
    private float elapsedTime = 0f;

    private void OnEnable()
    {
        OnReachedDestination += HandleReachedDestination;
    }

    private void OnDisable()
    {
        OnReachedDestination -= HandleReachedDestination;
    }

    private void HandleReachedDestination()
    {
        isFollowing = false;
    }

    private void UpdateSpeed()
    {
        elapsedTime += Time.deltaTime;
        agent.speed = maxSpeed - (maxSpeed - baseSpeed) * Mathf.Exp(-growthRate * elapsedTime);
    }

    private void HandleFollowThreshold(float distanceToPlayer)
    {
        if (!isFollowing)
        {
            if (distanceToPlayer > resumeFollowDistance)
            {
                isFollowing = true;
                lastKnownPlayerPosition = playerTransform.position;
                MoveTo(playerTransform.position);
            }
        }
        else
        {
            float playerMoved = Vector3.Distance(playerTransform.position, lastKnownPlayerPosition);
            if (playerMoved > agent.stoppingDistance)
            {
                lastKnownPlayerPosition = playerTransform.position;
                MoveTo(playerTransform.position);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        GameOverScreen.Instance?.Show();
    }
    protected override void Update()
    {
        base.Update();

        if (playerTransform == null) return;

        UpdateSpeed();

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        HandleFollowThreshold(distanceToPlayer);
    }
}