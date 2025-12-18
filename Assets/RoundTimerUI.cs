using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoundTimerUI : MonoBehaviour
{
    [Header("Timer")]
    public float roundLengthSeconds = 90f;
    public bool startOnPlay = true;

    [Header("UI (assign ONE of these)")]
    public TMP_Text tmpText;     // TextMeshPro
    public Text legacyText;      // Legacy UI Text

    private float _timeLeft;
    private bool _running;

    void Awake()
    {
        // Auto-find a text on the same object if not assigned
        if (tmpText == null) tmpText = GetComponent<TMP_Text>();
        if (legacyText == null) legacyText = GetComponent<Text>();
    }

    void Start()
    {
        _timeLeft = roundLengthSeconds;
        UpdateLabel();

        if (startOnPlay)
            _running = true;
    }

    void Update()
    {
        if (!_running) return;

        // Stop counting if game is already over
        if (SpyGameManager.Instance != null && SpyGameManager.Instance.gameOver)
        {
            _running = false;
            return;
        }

        _timeLeft -= Time.deltaTime;

        if (_timeLeft <= 0f)
        {
            _timeLeft = 0f;
            UpdateLabel();
            TimeUp();
            return;
        }

        UpdateLabel();
    }

    void UpdateLabel()
    {
        string s = FormatTime(_timeLeft);

        if (tmpText != null) tmpText.text = s;
        if (legacyText != null) legacyText.text = s;
    }

    string FormatTime(float seconds)
    {
        int total = Mathf.CeilToInt(seconds);
        int m = total / 60;
        int s = total % 60;
        return $"{m:00}:{s:00}";
    }

    void TimeUp()
    {
        _running = false;

        // End the round
        if (SpyGameManager.Instance != null)
        {
            SpyGameManager.Instance.gameOver = true;
            SpyGameManager.Instance.playerWon = false;

            // Optional: show who the spy was
            if (SpyGameManager.Instance.spy != null)
            {
                SpyGameManager.Instance.lastMessage =
                    $"TIME'S UP!\nYou hesitated.\nThe real spy was: {SpyGameManager.Instance.spy.npcName}.";
            }

            Debug.Log(SpyGameManager.Instance.lastMessage);
        }
        else
        {
            Debug.Log("TIME'S UP!");
        }
    }
}
