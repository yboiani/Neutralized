using UnityEngine;

public class SpyGameManager : MonoBehaviour
{
    public static SpyGameManager Instance;

    [Header("NPCs")]
    public NpcController[] npcs;
    public NpcController spy;

    [Header("Game State")]
    public bool gameOver = false;
    public bool playerWon = false;
    [TextArea]
    public string lastMessage = "";

    [Header("NPC Collider Settings")]
    [Tooltip("Height of the capsule collider for each NPC.")]
    public float colliderHeight = 1.8f;
    [Tooltip("Radius of the capsule collider for each NPC.")]
    public float colliderRadius = 0.28f;
    [Tooltip("Vertical center of the collider relative to NPC pivot.")]
    public float colliderCenterY = 0.9f;

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
        // Find all NPCs in the scene
        npcs = FindObjectsOfType<NpcController>();

        if (npcs.Length == 0)
        {
            Debug.LogError("SpyGameManager: no NPCs found in the scene!");
            return;
        }

        // Ensure each has a collider and a nice name
        for (int i = 0; i < npcs.Length; i++)
        {
            var npc = npcs[i];

            SetupNpcCollider(npc);

            if (string.IsNullOrEmpty(npc.npcName))
            {
                npc.npcName = $"NPC_{i + 1}";
            }

            npc.gameObject.name = npc.npcName;
        }

        // Randomly select the spy
        int spyIndex = Random.Range(0, npcs.Length);
        spy = npcs[spyIndex];
        spy.isSpy = true;
        spy.SetSpyVisual(false);   // remains visually normal for now

        Debug.Log($"[SpyGameManager] Spy is: {spy.npcName} (secret).");
    }

    void SetupNpcCollider(NpcController npc)
    {
        CapsuleCollider col = npc.GetComponent<CapsuleCollider>();
        if (col == null)
        {
            col = npc.gameObject.AddComponent<CapsuleCollider>();
        }

        col.direction = 1; // Y-axis
        col.height = colliderHeight;
        col.radius = colliderRadius;
        col.center = new Vector3(0f, colliderCenterY, 0f);
    }

    public void OnNpcShot(NpcController npc)
    {
        if (gameOver) return;

        if (npc == null)
        {
            lastMessage = "You missed… that wasn’t even an NPC.";
            Debug.Log(lastMessage);
            return;
        }

        gameOver = true;

        if (npc.isSpy)
        {
            playerWon = true;
            lastMessage = $"YOU WIN!\nYou correctly shot the spy: {npc.npcName}.";
        }
        else
        {
            playerWon = false;
            lastMessage = $"YOU LOSE!\n{npc.npcName} was innocent.\nThe real spy was: {spy.npcName}.";
        }

        Debug.Log(lastMessage);
    }
}
