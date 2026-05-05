using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI scoreText;

    private int score;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    [Header("Points par item")]
    [SerializeField] private int pointsPerBurger = 3;
    [SerializeField] private int pointsPerFries = 1;

    public void AddScore(int burgers, int fries)
    {
        score += burgers * pointsPerBurger + fries * pointsPerFries;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score : {score}";
    }
}
