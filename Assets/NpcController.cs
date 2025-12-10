using UnityEngine;

public class NpcController : MonoBehaviour
{
    [Header("Spy Info")]
    public bool isSpy = false;
    public string npcName = "NPC";

    [Header("Movement Settings")]
    public bool enableMovement = true;
    [Tooltip("How fast the NPC walks (units per second).")]
    public float moveSpeed = 1.5f;
    [Tooltip("How far from their start position they are allowed to roam.")]
    public float roamRadius = 2.0f;
    [Tooltip("Minimum time to wait between walks.")]
    public float minIdleTime = 1.0f;
    [Tooltip("Maximum time to wait between walks.")]
    public float maxIdleTime = 3.0f;

    private Vector3 _startPosition;
    private Vector3 _currentTarget;
    private float _idleTimer;

    private enum State { Idle, Walking }
    private State _state = State.Idle;

    void Start()
    {
        // starting position for roaming
        _startPosition = transform.position;
        PickNewIdleTime();
    }

    void Update()
    {
        if (!enableMovement) return;

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
        // keep movement on the ground plane
        Vector3 target = new Vector3(_currentTarget.x, transform.position.y, _currentTarget.z);

        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            moveSpeed * Time.deltaTime
        );

        // rotate to face movement direction
        Vector3 dir = target - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.0001f)
        {
            Quaternion look = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, 10f * Time.deltaTime);
        }

        if (Vector3.Distance(transform.position, target) < 0.05f)
        {
            PickNewIdleTime();
        }
    }

    void PickNewIdleTime()
    {
        _idleTimer = Random.Range(minIdleTime, maxIdleTime);
        _state = State.Idle;
    }

    void ChooseNewDestination()
    {
        Vector2 circle = Random.insideUnitCircle * roamRadius;
        _currentTarget = new Vector3(
            _startPosition.x + circle.x,
            _startPosition.y,
            _startPosition.z + circle.y
        );
        _state = State.Walking;
    }

    // Called by SpyGameManager when we want to visually mark the spy.
    // Right now it's a no-op; later you can add: change color, hat, etc.
    public void SetSpyVisual(bool show)
    {
        // intentionally left empty for now
    }

    // Called by the sniper script when this NPC gets shot
    public void OnShot()
    {
        enableMovement = false;
        // simplest possible reaction: disappear
        gameObject.SetActive(false);
    }
}
