using UnityEngine;
using UnityEngine.AI;

public class NpcController : MonoBehaviour
{
    [Header("Spy Info")]
    public bool isSpy = false;
    public string npcName = "NPC";

    [Header("Movement Settings")]
    public bool enableMovement = true;

    public float roamRadius = 6.0f;
    public float minIdleTime = 1.0f;
    public float maxIdleTime = 3.0f;
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
        PickNewIdleTimeSafe(); // SAFE version
    }

    void Update()
    {
        if (agent == null) return;

        if (!enableMovement)
        {
            if (agent.isOnNavMesh) agent.isStopped = true;
            return;
        }

        // If not on navmesh yet, keep trying to place it
        if (!agent.isOnNavMesh)
        {
            TryWarpToNavMesh();
            return;
        }

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
        if (!agent.isOnNavMesh) return;
        if (agent.pathPending) return;

        bool arrived =
            (!agent.hasPath) ||
            (agent.remainingDistance <= Mathf.Max(arriveDistance, agent.stoppingDistance));

        if (arrived)
        {
            PickNewIdleTimeSafe();
        }
    }

    void PickNewIdleTimeSafe()
    {
        _idleTimer = Random.Range(minIdleTime, maxIdleTime);
        _state = State.Idle;

        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
    }

    void ChooseNewDestination()
    {
        if (agent == null || !agent.isOnNavMesh) return;

        const int maxTries = 12;

        for (int i = 0; i < maxTries; i++)
        {
            Vector2 circle = Random.insideUnitCircle * roamRadius;
            Vector3 randomWorld = new Vector3(
                _startPosition.x + circle.x,
                _startPosition.y,
                _startPosition.z + circle.y
            );

            if (NavMesh.SamplePosition(randomWorld, out NavMeshHit hit, 3.0f, NavMesh.AllAreas))
            {
                agent.isStopped = false;
                agent.SetDestination(hit.position);
                _state = State.Walking;
                return;
            }
        }

        PickNewIdleTimeSafe();
    }

    void TryWarpToNavMesh()
    {
        if (agent == null) return;

        // Try to snap the agent onto the nearest navmesh point near its current position
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 5.0f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
            _startPosition = hit.position;
            PickNewIdleTimeSafe();
        }
    }

    public void SetSpyVisual(bool show) { }

    public void OnShot()
    {
        enableMovement = false;

        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }

        gameObject.SetActive(false);
    }
}
