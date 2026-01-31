using UnityEngine;
using TMPro;

public class ScoreUIBinder : MonoBehaviour
{
    public TextMeshProUGUI currentTMP;
    public TextMeshProUGUI bestTMP;

    private void Start()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.currentTMP = currentTMP;
            ScoreManager.Instance.bestTMP = bestTMP;

            ScoreManager.Instance.UpdateUIManually();
        }
    }
}