using UnityEngine;

public class ScopeUIController : MonoBehaviour
{
    [Header("UI")]
    public GameObject scopeUI;     // drag ScopeDark here
    public GameObject hipFireUI;   // drag HipFireUI here

    [Header("FOV")]
    public float zoomFOV = 20f;
    public float normalFOV = 40f;

    private Camera cam;
    private bool isScoped = false;

    void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam == null) cam = Camera.main;
    }

    void Start()
    {
        SetScoped(false);
    }

    void Update()
    {
        // HOLD right click to scope
        bool wantScope = Input.GetMouseButton(1);

        if (wantScope && !isScoped) SetScoped(true);
        else if (!wantScope && isScoped) SetScoped(false);
    }

    private void SetScoped(bool scoped)
    {
        isScoped = scoped;

        if (scopeUI != null) scopeUI.SetActive(scoped);
        if (hipFireUI != null) hipFireUI.SetActive(!scoped);

        if (cam != null) cam.fieldOfView = scoped ? zoomFOV : normalFOV;
    }
}
