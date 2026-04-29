using UnityEngine;

public class CompanionController : AIController
{
    public Transform playerTransform;
    public float resumeFollowDistance = 3f;

    private bool isFollowing = false;
    private Vector3 lastKnownPlayerPosition;

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

    protected override void Update()
    {
        base.Update();

        if (playerTransform == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        HandleFollowThreshold(distanceToPlayer);
    }
}