using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Timer")]
    public float levelTimeSeconds = 300f; // example
    private float timer;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        timer = levelTimeSeconds;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        UIManager.Instance?.UpdateTimer(Mathf.Max(0f, timer));
        if (timer <= 0f)
        {
            GameOver();
        }
    }

    public void LevelComplete()
    {
        Debug.Log("Level Complete!");
        // show win UI, stop time
        UIManager.Instance?.ShowLevelComplete();
        enabled = false;
    }

    public void GameOver()
    {
        Debug.Log("Game Over");
        UIManager.Instance?.ShowGameOver();
        enabled = false;
    }
}
