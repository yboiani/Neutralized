using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SimpleGameUI : MonoBehaviour
{
    [Header("UI Reference")]
    public TextMeshProUGUI messageText;

    void Start()
    {
        if (messageText != null)
        {
            messageText.text = "Find the spy and left-click to shoot.";
        }
    }

    void Update()
    {
        if (SpyGameManager.Instance == null || messageText == null)
            return;

        SpyGameManager gm = SpyGameManager.Instance;

        if (!gm.gameOver)
        {
            // Instructions while game running
            messageText.text = "Find the spy and left-click to shoot.";
        }
        else
        {
            // Show result and restart hint
            messageText.text = gm.lastMessage + "\n\nPress R to restart.";

            if (Input.GetKeyDown(KeyCode.R))
            {
                Scene current = SceneManager.GetActiveScene();
                SceneManager.LoadScene(current.buildIndex);
            }
        }
    }
}
