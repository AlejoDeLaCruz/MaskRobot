using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Score")]
    public float pointsPerSecond = 1f;
    public string playerPrefsKey = "HighScore";

    [HideInInspector] public TMPro.TextMeshProUGUI currentTMP;
    [HideInInspector] public TMPro.TextMeshProUGUI bestTMP;

    private float currentScore;
    private int bestScore;
    private bool running;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        LoadBest();
        ResetRun();
    }

    private void Update()
    {
        if (!running) return;

        currentScore += pointsPerSecond * Time.deltaTime;
        UpdateUI();
    }

    void UpdateUI()
    {
        int scoreInt = Mathf.FloorToInt(currentScore);

        if (currentTMP != null)
            currentTMP.text = $"Score: {scoreInt}";

        if (bestTMP != null)
            bestTMP.text = $"Record: {bestScore}";
    }

    public void GameOver()
    {
        running = false;
        TrySaveBest();
        UpdateUI();
    }

    public void UpdateUIManually()
    {
        UpdateUI();
    }

    public void ResetRun()
    {
        currentScore = 0f;
        running = true;
        UpdateUI();
    }

    void TrySaveBest()
    {
        int scoreInt = Mathf.FloorToInt(currentScore);

        if (scoreInt > bestScore)
        {
            bestScore = scoreInt;
            PlayerPrefs.SetInt(playerPrefsKey, bestScore);
            PlayerPrefs.Save();
        }
    }

    void LoadBest()
    {
        bestScore = PlayerPrefs.GetInt(playerPrefsKey, 0);
    }

    [ContextMenu("Reset HighScore")]
    public void ResetBest()
    {
        PlayerPrefs.DeleteKey(playerPrefsKey);
        bestScore = 0;
        UpdateUI();
    }
}