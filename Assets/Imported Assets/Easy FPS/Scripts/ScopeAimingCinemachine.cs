using UnityEngine;
using Cinemachine;
using TMPro;

public class ScopeAimingCinemachine : MonoBehaviour
{
    [Header("Cinemachine")]
    [SerializeField] private CinemachineVirtualCamera vcam;

    [Header("UI")]
    [SerializeField] private TMP_Text hipCrosshairText;
    [SerializeField] private GameObject scopeOverlay;

    [Header("FOV")]
    [SerializeField] private float normalFOV = 40f;
    [SerializeField] private float zoomFOV = 20f;
    [SerializeField] private float fovLerpSpeed = 15f;

    private void Awake()
    {
        // Force starting state
        ApplyState(false, true);
    }

    private void Update()
    {
        // HOLD right mouse button (NO toggle, NO edge cases)
        bool isAiming = Input.GetMouseButton(1);

        ApplyState(isAiming, false);
    }

    private void LateUpdate()
    {
        // Re-assert UI so other scripts can't fight us
        bool isAiming = Input.GetMouseButton(1);

        if (hipCrosshairText != null)
            hipCrosshairText.gameObject.SetActive(!isAiming);

        if (scopeOverlay != null)
            scopeOverlay.SetActive(isAiming);
    }

    private void ApplyState(bool aiming, bool instant)
    {
        // UI
        if (hipCrosshairText != null)
            hipCrosshairText.gameObject.SetActive(!aiming);

        if (scopeOverlay != null)
            scopeOverlay.SetActive(aiming);

        // FOV
        if (vcam == null) return;

        float targetFOV = aiming ? zoomFOV : normalFOV;

        var lens = vcam.m_Lens;
        lens.FieldOfView = instant
            ? targetFOV
            : Mathf.Lerp(lens.FieldOfView, targetFOV, Time.deltaTime * fovLerpSpeed);
        vcam.m_Lens = lens;
    }
}
