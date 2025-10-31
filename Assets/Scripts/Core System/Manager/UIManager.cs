using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("HP UI")]
    [SerializeField] private Image hpBarBackground;
    [SerializeField] private Image hpBarFill;
    [SerializeField] private TMP_Text hpText;

    [Header("Panels")]
    [SerializeField] private GameObject gameOverPanel;

    private void Awake()
    {
        Instance = this;
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    public void UpdateHP(int hp, int max)
    {
        if (hpBarFill != null)
            StartCoroutine(SmoothFill(hpBarFill.fillAmount, (float)hp / max));

        if (hpText != null)
            hpText.text = $"{hp} / {max}";
    }

    private IEnumerator SmoothFill(float start, float end)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 5f; // speed of lerp
            hpBarFill.fillAmount = Mathf.Lerp(start, end, t);
            yield return null;
        }
        hpBarFill.fillAmount = end;
    }

    public void ShowGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }
}
