using TMPro;
using UnityEngine;

public class FinalScoreDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;

    private void Awake()
    {
        if (scoreText == null)
            scoreText = GetComponent<TMP_Text>();

        if (scoreText == null) return;

        int score = PlayerScoreTracker.GetFinalScore();
        int deaths = PlayerScoreTracker.GetFinalDeathCount();
        scoreText.text = $"Final Score: {score}/100\nDeaths: {deaths}";
    }
}
