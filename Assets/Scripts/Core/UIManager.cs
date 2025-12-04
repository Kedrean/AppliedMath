using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI refs")]
    public Text timerText;
    public Text hpText;
    public Text livesText;
    public GameObject gameOverPanel;
    public GameObject levelCompletePanel;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void UpdateTimer(float seconds)
    {
        if (timerText == null) return;
        int s = Mathf.CeilToInt(seconds);
        timerText.text = $"Time: {s}";
    }

    public void UpdateHP(int current, int max)
    {
        if (hpText == null) return;
        hpText.text = $"HP: {current}/{max}";
    }

    public void UpdateLives(int lives)
    {
        if (livesText == null) return;
        livesText.text = $"Lives: {lives}";
    }

    public void ShowGameOver()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    public void ShowLevelComplete()
    {
        if (levelCompletePanel != null) levelCompletePanel.SetActive(true);
    }
}
