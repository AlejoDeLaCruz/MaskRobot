using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Score")]
    public float pointsPerSecond = 1f;
    public string playerPrefsKey = "HighScore";

    [HideInInspector]
    public TMPro.TextMeshProUGUI currentTMP;
    [HideInInspector]
    public TMPro.TextMeshProUGUI bestTMP;

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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reiniciar el juego cuando se recarga la escena
        ResetRun();

        // Buscar y reconectar las referencias UI después de cargar la escena
        FindUIReferences();
        UpdateUI();
    }

    private void Start()
    {
        LoadBest();
        ResetRun();
        FindUIReferences();
    }

    private void Update()
    {
        if (!running) return;

        currentScore += pointsPerSecond * Time.deltaTime;
        UpdateUI();
    }

    void FindUIReferences()
    {
        // Buscar los TextMeshPro por nombre o tag
        GameObject currentObj = GameObject.Find("CurrentScoreText");
        GameObject bestObj = GameObject.Find("BestScoreText");

        if (currentObj != null)
            currentTMP = currentObj.GetComponent<TMPro.TextMeshProUGUI>();

        if (bestObj != null)
            bestTMP = bestObj.GetComponent<TMPro.TextMeshProUGUI>();
    }

    void UpdateUI()
    {
        int scoreInt = Mathf.FloorToInt(currentScore);

        if (currentTMP != null)
            currentTMP.text = $"{scoreInt}";

        if (bestTMP != null)
            bestTMP.text = $"{bestScore}";
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

    public void SetUIReferences(TMPro.TextMeshProUGUI current, TMPro.TextMeshProUGUI best)
    {
        currentTMP = current;
        bestTMP = best;
        UpdateUI();
    }
}