using UnityEngine;

[DisallowMultipleComponent]
public class NpcAnimationOffset : MonoBehaviour
{
    public Animator animator;
    public bool randomizeStartTime = true;

    public float speedMin = 0.9f;
    public float speedMax = 1.1f;

    public string stateName = "";
    public int layer = 0;

    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (animator == null) return;

        animator.speed = Random.Range(speedMin, speedMax);

        if (!randomizeStartTime) return;

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
}