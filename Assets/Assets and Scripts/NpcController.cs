using UnityEngine;
using UnityEngine.AI;

public class NpcController : MonoBehaviour
{
    [Header("Spy Info")]
    public bool isSpy = false;
    public string npcName = "NPC";

    [Header("Movement Settings")]
    public bool enableMovement = true;

    [Tooltip("How far from their start position they are allowed to roam.")]
    public float roamRadius = 6.0f;

    [Tooltip("Minimum time to wait between walks.")]
    public float minIdleTime = 1.0f;

    [Tooltip("Maximum time to wait between walks.")]
    public float maxIdleTime = 3.0f;

    [Tooltip("How close the agent must be to consider destination reached.")]
    public float arriveDistance = 0.25f;

    [Header("NavMesh")]
    public NavMeshAgent agent;

    private Vector3 _startPosition;
    private float _idleTimer;

    private enum State { Idle, Walking }
    private State _state = State.Idle;

    void Awake()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        _startPosition = transform.position;
        PickNewIdleTime();
    }

    void Update()
    {
        if (!enableMovement)
        {
            if (agent != null) agent.isStopped = true;
            return;
        }

        if (agent == null || !agent.isOnNavMesh)
            return;

        switch (_state)
        {
            case State.Idle:
                HandleIdle();
                break;

            case State.Walking:
                HandleWalking();
                break;
        }
    }

    void HandleIdle()
    {
        _idleTimer -= Time.deltaTime;
        if (_idleTimer <= 0f)
        {
            ChooseNewDestination();
        }
    }

    void HandleWalking()
    {
        // If agent has no path yet, just wait a moment
        if (agent.pathPending) return;

        // Consider arrived when close enough OR no path remaining
        bool arrived =
            (!agent.hasPath) ||
            (agent.remainingDistance <= Mathf.Max(arriveDistance, agent.stoppingDistance));

        if (arrived)
        {
            PickNewIdleTime();
        }
    }

    void PickNewIdleTime()
    {
        _idleTimer = Random.Range(minIdleTime, maxIdleTime);
        _state = State.Idle;

        if (agent != null)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
    }

    void ChooseNewDestination()
    {
        // Try a few random samples until we find a valid NavMesh point
        const int maxTries = 12;

        for (int i = 0; i < maxTries; i++)
        {
            Vector2 circle = Random.insideUnitCircle * roamRadius;
            Vector3 randomWorld = new Vector3(
                _startPosition.x + circle.x,
                _startPosition.y,
                _startPosition.z + circle.y
            );

            // Snap to navmesh
            if (NavMesh.SamplePosition(randomWorld, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
            {
                agent.isStopped = false;
                agent.SetDestination(hit.position);
                _state = State.Walking;
                return;
            }
        }

        // If we failed to find a point, just idle again
        PickNewIdleTime();
    }

    // Later: highlight spy, outline, etc.
    public void SetSpyVisual(bool show)
    {
        // intentionally empty
    }

    public void OnShot()
    {
        enableMovement = false;

        if (agent != null)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }

        gameObject.SetActive(false);
    }
}
