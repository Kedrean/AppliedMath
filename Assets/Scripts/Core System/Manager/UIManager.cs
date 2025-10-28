using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance {  get; private set; }

    [SerializeField] private Slider hpSlider;
    [SerializeField] private Text hpText;
    [SerializeField] private GameObject gameOverPanel;

    private void Awake()
    {
        Instance = this;
        gameOverPanel?.SetActive(false);
    }

    public void UpdateHP(int hp, int max)
    {
        if (hpSlider != null)
            hpSlider.value = (float)hp / max;
        if (hpText != null)
            hpText.text = $"{hp} / {max}";
    }

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
    }
}
