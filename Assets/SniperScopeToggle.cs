using UnityEngine;

public class SniperScopeToggle : MonoBehaviour
{
    public Camera mainCamera;     // Your normal gameplay camera
    public Camera sniperCamera;   // Your zoom/sniper camera

    private bool isScoped = false;

    void Start()
    {
        // Make sure we start in normal view
        if (mainCamera != null)
            mainCamera.enabled = true;

        if (sniperCamera != null)
            sniperCamera.enabled = false;
    }

    void Update()
    {
        // Right click (mouse button 1)
        if (Input.GetMouseButtonDown(1))
        {
            ToggleScope();
        }
    }

    void ToggleScope()
    {
        isScoped = !isScoped;

        // Switch cameras
        if (mainCamera != null)
            mainCamera.enabled = !isScoped;

        if (sniperCamera != null)
            sniperCamera.enabled = isScoped;
    }
}
