using TMPro;
using UnityEngine;

public class ScoreTextUpdater : MonoBehaviour
{
    public string FormatString = "Score: {0}";
    public TextMeshProUGUI ScoreText;

    private void Awake()
    {
        if (GameController.Current)
        {
            UpdateScore(GameController.Current.Score);
        }
    }

    void Start()
    {
        GameController.Current.OnScoreUpdated += (s, e) => UpdateScore(e);
        ScoreText = GetComponent<TextMeshProUGUI>();
    }

    private void UpdateScore(int newScore)
    {
        var scoreString = string.Format(FormatString, newScore);
        ScoreText.SetText(scoreString);
    }
}
