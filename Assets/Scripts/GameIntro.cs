using UnityEngine;
using System.Collections.Generic;

// attach to a GameObject in SampleScene
// covers Task 3 storyline foundation requirement
public class GameIntro : MonoBehaviour
{
    private void Start()
    {
        var story = new List<string>
        {
            "The Exile — a warrior cast out from the City.",
            "Betrayed by Solomon Asantey, the city's ruler.",
            "Travel through five biomes. Defeat five bosses.",
            "Reclaim your home. Press Z to advance."
        };

        // slight delay so DialogManager has time to Awake first
        StartCoroutine(ShowIntroAfterDelay(story));
    }

    private System.Collections.IEnumerator ShowIntroAfterDelay(List<string> story)
    {
        yield return null; // wait one frame
        if (DialogManager.Instance != null)
            DialogManager.Instance.ShowDialog(story);
        else
            Debug.LogWarning("[GameIntro] DialogManager not found in scene.");
    }
}