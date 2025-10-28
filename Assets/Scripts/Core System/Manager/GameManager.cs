using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private bool _gameOver;

    private void Awaike() => Instance = this;

    public void OnPlayerDead()
    {
        if (_gameOver) return;
        _gameOver = true;
        Time.timeScale = 0f;
        UIManager.Instance.ShowGameOver();
    }
}
