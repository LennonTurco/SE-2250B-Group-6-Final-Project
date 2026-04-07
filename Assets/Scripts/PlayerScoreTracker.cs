using UnityEngine;

public static class PlayerScoreTracker
{
    private const string DeathCountKey = "PlayerDeathCount";
    private const string FinalDeathCountKey = "FinalPlayerDeathCount";
    private const string FinalScoreKey = "FinalPlayerScore";
    private const int MaxScore = 100;
    private const int DeathPenalty = 5;

    public static void ResetScore()
    {
        PlayerPrefs.SetInt(DeathCountKey, 0);
        PlayerPrefs.DeleteKey(FinalDeathCountKey);
        PlayerPrefs.DeleteKey(FinalScoreKey);
        PlayerPrefs.Save();
    }

    public static void RecordDeath()
    {
        int deaths = GetDeathCount() + 1;
        PlayerPrefs.SetInt(DeathCountKey, deaths);
        PlayerPrefs.Save();
    }

    public static int GetDeathCount()
    {
        return Mathf.Max(0, PlayerPrefs.GetInt(DeathCountKey, 0));
    }

    public static int GetCurrentScore()
    {
        return CalculateScore(GetDeathCount());
    }

    public static void SaveFinalScore()
    {
        int deaths = GetDeathCount();
        PlayerPrefs.SetInt(FinalDeathCountKey, deaths);
        PlayerPrefs.SetInt(FinalScoreKey, CalculateScore(deaths));
        PlayerPrefs.Save();
    }

    public static int GetFinalDeathCount()
    {
        return Mathf.Max(0, PlayerPrefs.GetInt(FinalDeathCountKey, GetDeathCount()));
    }

    public static int GetFinalScore()
    {
        return Mathf.Clamp(PlayerPrefs.GetInt(FinalScoreKey, GetCurrentScore()), 0, MaxScore);
    }

    public static void ClearPrefsButKeepFinalScore()
    {
        int finalDeaths = GetFinalDeathCount();
        int finalScore = GetFinalScore();

        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt(FinalDeathCountKey, finalDeaths);
        PlayerPrefs.SetInt(FinalScoreKey, finalScore);
        PlayerPrefs.Save();
    }

    private static int CalculateScore(int deaths)
    {
        return Mathf.Clamp(MaxScore - (Mathf.Max(0, deaths) * DeathPenalty), 0, MaxScore);
    }
}
