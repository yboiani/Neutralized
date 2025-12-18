using StarterAssets;
using UnityEngine;

public class PauseMenuUI : MonoBehaviour
{
    public GameObject pauseCanvas;
    public GameObject instructionsPanel;
    public GameObject settingsPanel;
    public ThirdPersonController player;

    void Start()
    {
        pauseCanvas.SetActive(false);
        instructionsPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    void Update()
    {
        if (player.IsPaused && !pauseCanvas.activeSelf)
            pauseCanvas.SetActive(true);

        if (!player.IsPaused && pauseCanvas.activeSelf)
            pauseCanvas.SetActive(false);
    }

    // ===== MAIN MENU BUTTONS ======

    public void ResumeGame()
    {
        player.IsPaused = false;
    }

    public void OpenInstructions()
    {
        instructionsPanel.SetActive(true);
        pauseCanvas.SetActive(false);
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        pauseCanvas.SetActive(false);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif

        Application.Quit();
        Debug.Log("Quit Game");
    }

    // ===== SUBMENU BACK BUTTONS =====

    public void BackFromInstructions()
    {
        instructionsPanel.SetActive(false);
        pauseCanvas.SetActive(true);
    }

    public void BackFromSettings()
    {
        settingsPanel.SetActive(false);
        pauseCanvas.SetActive(true);
    }
}
