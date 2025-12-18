using UnityEngine;

public class NPCStationAnimator : MonoBehaviour
{
    public Animator animator;

    [Header("Pick one state at Start")]
    public string[] stationStates =
    {
        "HumanM@Talk01",
        "HumanM@Talk02",
        "HumanM@Talk03",
        "HumanM@Question01",
        "HumanM@Question02",
        "HumanM@HeadNod01",
        "HumanM@HeadShake01",
        "HumanM@HeadShake02"
    };

    [Range(0f, 0.25f)]
    public float crossFadeTime = 0.05f;

    [Tooltip("Randomize normalized time so they aren't synced")]
    public bool randomizeStartTime = true;

    void Awake()
    {
        if (!animator) animator = GetComponent<Animator>();
    }

    void Start()
    {
        if (!animator || stationStates == null || stationStates.Length == 0) return;

        string pick = stationStates[Random.Range(0, stationStates.Length)];
        float t = randomizeStartTime ? Random.value : 0f;

        animator.Play(pick, 0, t);
        animator.Update(0f);
    }
}
