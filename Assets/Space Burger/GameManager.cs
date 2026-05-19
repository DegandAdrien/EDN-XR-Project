using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Paramètres de partie")]
    [SerializeField] private float gameDuration = 600f;
    [SerializeField] private int maxErrors = 5;

    [Header("Difficulté")]
    [SerializeField] private float minPatienceMultiplier = 0.4f;

    [Header("UI - En jeu")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI errorText;

    [Header("UI - Game Over")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverScoreText;
    [SerializeField] private TextMeshProUGUI gameOverErrorText;
    [SerializeField] private TextMeshProUGUI restartCountdownText;
    [SerializeField] private float restartDelay = 10f;

    private float remainingTime;
    private int errorCount;
    private bool isGameOver;
    private float restartCountdown;

    public bool IsGameOver => isGameOver;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        remainingTime = gameDuration;
        errorCount = 0;
        isGameOver = false;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        UpdateInGameUI();
    }

    private void Update()
    {
        if (isGameOver)
        {
            restartCountdown -= Time.deltaTime;

            if (restartCountdownText != null)
                restartCountdownText.text = $"Nouvelle partie dans : {Mathf.CeilToInt(restartCountdown)}s";

            if (restartCountdown <= 0f)
                RestartGame();

            return;
        }

        remainingTime -= Time.deltaTime;
        UpdateInGameUI();

        if (remainingTime <= 0f)
            TriggerGameOver();
    }

    public void RegisterError()
    {
        if (isGameOver) return;

        errorCount++;
        UpdateInGameUI();

        if (errorCount >= maxErrors)
            TriggerGameOver();
    }

    public float GetDifficultyMultiplier()
    {
        float progress = 1f - Mathf.Clamp01(remainingTime / gameDuration);
        return Mathf.Lerp(1f, minPatienceMultiplier, progress);
    }

    private void TriggerGameOver()
    {
        isGameOver = true;
        restartCountdown = restartDelay;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        int score = ScoreManager.Instance != null ? ScoreManager.Instance.Score : 0;

        if (gameOverScoreText != null)
            gameOverScoreText.text = $"Score : {score}";

        if (gameOverErrorText != null)
            gameOverErrorText.text = $"Clients perdus : {errorCount}/{maxErrors}";
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void UpdateInGameUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(remainingTime / 60f);
            int seconds = Mathf.FloorToInt(remainingTime % 60f);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }

        if (errorText != null)
            errorText.text = $"Erreurs : {errorCount}/{maxErrors}";
    }
}
