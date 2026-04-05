using UnityEngine;

public static class JungleProgressState
{
    private const string SkeletonHintSeenKey = "JungleSkeletonHintSeen";

    public static bool HasSeenSkeletonHint()
    {
        return PlayerPrefs.GetInt(SkeletonHintSeenKey, 0) == 1;
    }

    public static void MarkSkeletonHintSeen()
    {
        PlayerPrefs.SetInt(SkeletonHintSeenKey, 1);
        PlayerPrefs.Save();
    }
}
