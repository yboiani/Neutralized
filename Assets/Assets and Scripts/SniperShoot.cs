using UnityEngine;

public class SniperShoot : MonoBehaviour
{
    [Header("Shooting Settings")]
    public float range = 200f;
    public float fireCooldown = 0.3f;

    [Header("References")]
    public Camera cam;

    private float _cooldownTimer = 0f;

    void Awake()
    {
        if (cam == null)
        {
            cam = GetComponent<Camera>();
        }
    }

    void Update()
    {
        // If game is over, no more shooting
        if (SpyGameManager.Instance != null && SpyGameManager.Instance.gameOver)
            return;

        _cooldownTimer -= Time.deltaTime;

        if (Input.GetMouseButtonDown(0) && _cooldownTimer <= 0f)
        {
            _cooldownTimer = fireCooldown;
            Shoot();
        }
    }

    void Shoot()
    {
        if (cam == null)
        {
            Debug.LogWarning("SniperShoot: No camera assigned.");
            return;
        }

        // Ray from the center of the screen
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            // See if we hit an NPC
            NpcController npc = hit.collider.GetComponentInParent<NpcController>();

            if (npc != null)
            {
                npc.OnShot();
                if (SpyGameManager.Instance != null)
                {
                    SpyGameManager.Instance.OnNpcShot(npc);
                }
            }
            else
            {
                Debug.Log("Hit something that is not an NPC: " + hit.collider.name);
            }
        }
        else
        {
            Debug.Log("Shot missed, raycast hit nothing.");
        }
    }
}
