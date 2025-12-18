using UnityEngine;

[DisallowMultipleComponent]
public class NpcAnimationOffset : MonoBehaviour
{
    [Header("Optional (auto-finds if empty)")]
    public Animator animator;

    [Header("Desync")]
    public bool randomizeStartTime = true;

    [Header("Speed Variation")]
    public float speedMin = 0.9f;
    public float speedMax = 1.1f;

    [Header("Optional: state to offset (leave blank to offset current state)")]
    public string stateName = "";
    public int layer = 0;

    void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
    }

    void Start()
    {
        if (animator == null) return;

        // Force unique random per NPC (prevents "everyone same" at Play)
        int seed = GetInstanceID() ^ (int)(Time.realtimeSinceStartup * 1000f);
        var oldState = Random.state;
        Random.InitState(seed);

        animator.speed = Random.Range(speedMin, speedMax);

        if (randomizeStartTime)
        {
            float t = Random.value;

            if (string.IsNullOrEmpty(stateName))
            {
                var st = animator.GetCurrentAnimatorStateInfo(layer);
                animator.Play(st.shortNameHash, layer, t);
            }
            else
            {
                animator.Play(stateName, layer, t);
            }

            animator.Update(0f);
        }

        // Restore global random state so other systems stay normal
        Random.state = oldState;
    }
}
