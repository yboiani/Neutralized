using StarterAssets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    public GameObject pauseCanvas;
    public ThirdPersonController player;

    private void Start()
    {
        pauseCanvas.SetActive(false);
    }

    private void Update()
    {
        // Listen for ESC handled by the player script
        if (player.IsPaused && !pauseCanvas.activeSelf)
        {
            ShowPauseMenu();
        }
        else if (!player.IsPaused && pauseCanvas.activeSelf)
        {
            HidePauseMenu();
        }
    }

    public void ShowPauseMenu()
    {
        pauseCanvas.SetActive(true);
    }

    public void HidePauseMenu()
    {
        pauseCanvas.SetActive(false);
    }

    public void ResumeGame()
    {

        player.ResumeGame();     // <<< CALLS THE FUNCTION THAT LOCKS THE CURSOR
        pauseCanvas.SetActive(false);
        Time.timeScale = 1f;
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }
}
