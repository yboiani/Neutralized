using System.Collections;
using UnityEngine;

public class SpyRandomizer : MonoBehaviour
{
    [Header("All NPCs in the scene (drag them in)")]
    public NpcController[] npcs;

    [Header("Spy action timing (seconds)")]
    public float minActionDelay = 6f;
    public float maxActionDelay = 12f;

    [Header("Spy action triggers (must match Animator trigger names)")]
    public string[] spyTriggers = { "DoPeek", "DoPhone", "DoBug" };

    private NpcController _spy;
    private Animator _spyAnimator;

    void Start()
    {
        // If you forgot to drag them in, auto-find
        if (npcs == null || npcs.Length == 0)
            npcs = FindObjectsOfType<NpcController>();

        if (npcs.Length == 0)
        {
            Debug.LogError("SpyRandomizer: No NpcController objects found.");
            return;
        }

        // Pick ONE spy randomly
        _spy = npcs[Random.Range(0, npcs.Length)];
        _spy.isSpy = true;

        _spyAnimator = _spy.GetComponent<Animator>();
        if (_spyAnimator == null)
        {
            Debug.LogError("SpyRandomizer: Spy NPC has no Animator.");
            return;
        }

        // OPTIONAL: stop movement for everyone (since you said no time)
        foreach (var n in npcs)
            n.enableMovement = false;

        StartCoroutine(SpyRoutine());
        Debug.Log("SPY PICKED: " + _spy.name);
    }

    IEnumerator SpyRoutine()
    {
        while (true)
        {
            float wait = Random.Range(minActionDelay, maxActionDelay);
            yield return new WaitForSeconds(wait);

            if (_spyAnimator == null || spyTriggers.Length == 0) continue;

            string trig = spyTriggers[Random.Range(0, spyTriggers.Length)];
            _spyAnimator.SetTrigger(trig);
        }
    }
}
