using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

public class SniperScopeFOV : MonoBehaviour
{
    public CinemachineVirtualCamera playerVcam;
    public float normalFOV = 40f;
    public float scopedFOV = 5f;   // more zoom
    public float zoomSpeed = 10f;

    public GameObject scopeOverlay;   // <-- NEW

    private bool isScoped = false;
    private float currentFOV;

    void Start()
    {
        if (playerVcam != null)
        {
            currentFOV = playerVcam.m_Lens.FieldOfView;
            normalFOV = currentFOV;
        }

        if (scopeOverlay != null)
            scopeOverlay.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isScoped = !isScoped;

            if (scopeOverlay != null)
                scopeOverlay.SetActive(isScoped); // <-- NEW
        }

        if (playerVcam != null)
        {
            float targetFOV = isScoped ? scopedFOV : normalFOV;

            currentFOV = Mathf.Lerp(currentFOV, targetFOV, Time.deltaTime * zoomSpeed);

            var lens = playerVcam.m_Lens;
            lens.FieldOfView = currentFOV;
            playerVcam.m_Lens = lens;
        }
    }
}
