using UnityEngine;
using Cinemachine;

public class ScopeAimController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CinemachineVirtualCamera playerFollowVcam;
    [SerializeField] private GameObject hipFireUI;   // HipFireUI root
    [SerializeField] private GameObject scopeUI;     // ScopeUI root

    [Header("Zoom Settings")]
    [SerializeField] private float normalFov = 40f;
    [SerializeField] private float scopedFov = 15f;

    [Header("Controls")]
    [SerializeField] private KeyCode aimKey = KeyCode.Mouse1;

    private bool isAiming;

    private void Start()
    {
        SetAiming(false);
    }

    private void Update()
    {
        bool aimHeld = Input.GetKey(aimKey);

        // Only update when state changes (prevents spam)
        if (aimHeld != isAiming)
        {
            SetAiming(aimHeld);
        }
    }

    private void SetAiming(bool aiming)
    {
        isAiming = aiming;

        // UI swap
        if (hipFireUI != null) hipFireUI.SetActive(!aiming);
        if (scopeUI != null) scopeUI.SetActive(aiming);

        // Zoom (Cinemachine)
        if (playerFollowVcam != null)
        {
            playerFollowVcam.m_Lens.FieldOfView = aiming ? scopedFov : normalFov;
        }
    }
}
