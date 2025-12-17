using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    public Camera mainCamera;
    public TMP_Text scoreText;
    public TMP_Text timerText;

    [Header("Gameplay Settings")]
    public float startTimeSeconds = 60f;

    private int score = 0;
    private float timeRemaining;
    private bool isGameOver = false;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        timeRemaining = startTimeSeconds;

        UpdateScoreText();
        UpdateTimerText();
    }

    void Update()
    {
        if (isGameOver)
            return;

        // Handle timer
        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            isGameOver = true;
            UpdateTimerText();
            return;
        }

        UpdateTimerText();

        // Handle shooting
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            if (hit.collider.CompareTag("Target"))
            {
                Destroy(hit.collider.gameObject);
                score++;
                UpdateScoreText();
            }
        }
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    void UpdateTimerText()
    {
        if (timerText != null)
        {
            int seconds = Mathf.CeilToInt(timeRemaining);
            timerText.text = "Time: " + seconds;
        }
    }
}