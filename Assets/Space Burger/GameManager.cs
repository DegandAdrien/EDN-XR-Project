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
    [SerializeField] private float rushThreshold = 180f; // 3 dernières minutes

    [Header("UI - En jeu")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI errorText;
    [SerializeField] private TextMeshProUGUI rushText;

    [Header("UI - Game Over")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverScoreText;
    [SerializeField] private TextMeshProUGUI gameOverErrorText;
    [SerializeField] private TextMeshProUGUI restartCountdownText;
    [SerializeField] private float restartDelay = 10f;

    [Header("Audio - Musiques")]
    [SerializeField] private AudioSource bgMusicSource;
    [SerializeField] private AudioSource rushMusicSource;

    [Header("Audio - Effets")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip gameStartClip;
    [SerializeField] private AudioClip gameOverClip;

    private float remainingTime;
    private int errorCount;
    private bool isGameOver;
    private bool isRush;
    private float restartCountdown;

    public bool IsGameOver => isGameOver;
    public bool IsRush => isRush;

    // Retourne la progression de 0 (début) à 1 (fin)
    public float GetGameProgress() => 1f - Mathf.Clamp01(remainingTime / gameDuration);

    // Retourne un multiplicateur de patience entre 1 (début) et minPatienceMultiplier (fin)
    public float GetDifficultyMultiplier()
    {
        return Mathf.Lerp(1f, minPatienceMultiplier, GetGameProgress());
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Cacher dès Awake pour éviter le flash d'une frame
        if (rushText != null)
            rushText.gameObject.SetActive(false);
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    private void Start()
    {
        remainingTime = gameDuration;
        errorCount = 0;
        isGameOver = false;
        isRush = false;

        if (bgMusicSource != null) bgMusicSource.Play();
        if (sfxSource != null && gameStartClip != null) sfxSource.PlayOneShot(gameStartClip);

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
        UpdateRushState();
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

    private void UpdateRushState()
    {
        bool shouldRush = remainingTime <= rushThreshold;
        if (shouldRush == isRush) return;

        isRush = shouldRush;
        if (rushText != null)
            rushText.gameObject.SetActive(isRush);

        if (isRush)
        {
            if (bgMusicSource != null) bgMusicSource.Stop();
            if (rushMusicSource != null) rushMusicSource.Play();
        }
        else
        {
            if (rushMusicSource != null) rushMusicSource.Stop();
            if (bgMusicSource != null) bgMusicSource.Play();
        }
    }

    private void TriggerGameOver()
    {
        isGameOver = true;
        isRush = false;
        restartCountdown = restartDelay;

        if (bgMusicSource != null) bgMusicSource.Stop();
        if (rushMusicSource != null) rushMusicSource.Stop();
        if (sfxSource != null && gameOverClip != null) sfxSource.PlayOneShot(gameOverClip);

        if (rushText != null)
            rushText.gameObject.SetActive(false);

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
