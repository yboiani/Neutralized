using UnityEngine;

public class SpyGameManager : MonoBehaviour
{
    public static SpyGameManager Instance;

    [Header("NPCs (auto-filled)")]
    public NpcController[] npcs;
    public NpcController spy;

    [Header("Animator Controllers")]
    [Tooltip("Normal NPC controller (ex: NPC_Idles). If empty, uses whatever NPC already has.")]
    public RuntimeAnimatorController npcIdleController;

    [Tooltip("Spy-only controller (ex: NPC_Spy). REQUIRED for spy actions.")]
    public RuntimeAnimatorController spyController;

    [Header("Presentation Mode (Freeze NPCs)")]
    public bool freezeAllNpcMovement = true;

    [Header("Spy Actions (Animations)")]
    public bool enableSpyActions = true;
    public float minActionDelay = 6f;
    public float maxActionDelay = 12f;

    [Tooltip("These must exist as TRIGGER parameters in the SPY controller.")]
    public string triggerPeek = "DoPeek";
    public string triggerPhone = "DoPhone";
    public string triggerBug = "DoBug";

    [Header("Game State")]
    public bool gameOver = false;
    public bool playerWon = false;
    [TextArea] public string lastMessage = "";

    [Header("NPC Collider Settings")]
    public float colliderHeight = 1.8f;
    public float colliderRadius = 0.28f;
    public float colliderCenterY = 0.9f;

    private float _nextSpyActionTime = -1f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        // 1) Find all NPCs
        npcs = FindObjectsOfType<NpcController>();

        if (npcs == null || npcs.Length == 0)
        {
            Debug.LogError("SpyGameManager: no NPCs found (NpcController) in the scene!");
            return;
        }

        // 2) Setup colliders + optional freeze
        for (int i = 0; i < npcs.Length; i++)
        {
            var npc = npcs[i];
            if (npc == null) continue;

            SetupNpcCollider(npc);

            if (string.IsNullOrEmpty(npc.npcName))
                npc.npcName = $"NPC_{i + 1}";

            npc.gameObject.name = npc.npcName;

            if (freezeAllNpcMovement)
                npc.enableMovement = false;

            // If you want to FORCE everyone to use the idle controller at start:
            if (npcIdleController != null)
            {
                var a = npc.GetComponent<Animator>();
                if (a != null) a.runtimeAnimatorController = npcIdleController;
            }

            npc.isSpy = false; // reset
        }

        // 3) Pick spy at random
        int spyIndex = Random.Range(0, npcs.Length);
        spy = npcs[spyIndex];
        spy.isSpy = true;
        spy.SetSpyVisual(false);

        // 4) Swap ONLY the spy's controller to NPC_Spy
        if (spyController != null)
        {
            var spyAnim = spy.GetComponent<Animator>();
            if (spyAnim != null)
                spyAnim.runtimeAnimatorController = spyController;
            else
                Debug.LogWarning("SpyGameManager: Spy has no Animator component.");
        }
        else
        {
            Debug.LogWarning("SpyGameManager: spyController is NOT assigned, so spy actions won't work.");
        }

        Debug.Log($"[SpyGameManager] Spy chosen: {spy.npcName} (secret).");

        // 5) Schedule first spy action
        if (enableSpyActions)
            ScheduleNextSpyAction();
    }

    void Update()
    {
        if (!enableSpyActions) return;
        if (gameOver) return;
        if (spy == null) return;

        if (Time.time >= _nextSpyActionTime)
        {
            DoRandomSpyAction();
            ScheduleNextSpyAction();
        }
    }

    void ScheduleNextSpyAction()
    {
        float delay = Random.Range(minActionDelay, maxActionDelay);
        _nextSpyActionTime = Time.time + delay;
    }

    void DoRandomSpyAction()
    {
        var anim = spy.GetComponent<Animator>();
        if (anim == null) return;

        // Pick one of the three actions
        int r = Random.Range(0, 3);

        string trig = (r == 0) ? triggerPeek : (r == 1) ? triggerPhone : triggerBug;

        // Fire trigger (must exist on the spy controller)
        anim.ResetTrigger(triggerPeek);
        anim.ResetTrigger(triggerPhone);
        anim.ResetTrigger(triggerBug);

        anim.SetTrigger(trig);
    }

    void SetupNpcCollider(NpcController npc)
    {
        CapsuleCollider col = npc.GetComponent<CapsuleCollider>();
        if (col == null) col = npc.gameObject.AddComponent<CapsuleCollider>();

        col.direction = 1; // Y axis
        col.height = colliderHeight;
        col.radius = colliderRadius;
        col.center = new Vector3(0f, colliderCenterY, 0f);
    }

    public void OnNpcShot(NpcController npc)
    {
        if (gameOver) return;

        gameOver = true;

        if (npc != null && npc.isSpy)
        {
            playerWon = true;
            lastMessage = $"YOU WIN!\nYou correctly shot the spy: {npc.npcName}.";
        }
        else
        {
            playerWon = false;
            string shotName = (npc == null) ? "Nothing" : npc.npcName;
            lastMessage = $"YOU LOSE!\nYou shot: {shotName}.\nThe real spy was: {spy.npcName}.";
        }

        Debug.Log(lastMessage);
    }
}
