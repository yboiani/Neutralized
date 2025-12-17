using UnityEngine;
using Cinemachine;

public class SniperScopeFOV : MonoBehaviour
{
    public CinemachineVirtualCamera playerVcam;

    public float normalFOV = 40f;
    public float scopedFOV = 5f;
    public float zoomSpeed = 10f;

    public GameObject scopeOverlay;     // scope image
    public GameObject scopeVignette;    // NEW dark layer

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

        if (scopeVignette != null)
            scopeVignette.SetActive(false);
    }

    void Update()
    {
        // HOLD right click (NOT toggle)
        bool isScoped = Input.GetMouseButton(1);

        if (scopeOverlay != null)
            scopeOverlay.SetActive(isScoped);

        if (scopeVignette != null)
            scopeVignette.SetActive(isScoped);

        if (playerVcam != null)
        {
            float targetFOV = isScoped ? scopedFOV : normalFOV;

            currentFOV = Mathf.Lerp(
                currentFOV,
                targetFOV,
                Time.deltaTime * zoomSpeed
            );

            var lens = playerVcam.m_Lens;
            lens.FieldOfView = currentFOV;
            playerVcam.m_Lens = lens;
        }
    }
}
