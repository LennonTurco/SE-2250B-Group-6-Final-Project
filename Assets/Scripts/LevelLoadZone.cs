using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

// attach to an empty GO with a BoxCollider2D set to Is Trigger
// set the scene name in the inspector
public class LevelLoadZone : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;

    // optional - only allow loading if puzzle is solved
    [SerializeField] private bool requiresPuzzleSolved = false;
    [SerializeField] private bool requiresJungleSkeletonHint = false;

    [SerializeField] private List<string> blockedLines = new List<string>
    {
        "You can't leave yet. Finish what you started."
    };

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (string.IsNullOrEmpty(sceneToLoad)) return;

        if (requiresPuzzleSolved && PuzzleManager.Instance != null && !PuzzleManager.Instance.IsSolved())
        {
            DialogManager.Instance?.ShowDialog(blockedLines);
            return;
        }

        if (requiresJungleSkeletonHint && !JungleProgressState.HasSeenSkeletonHint())
        {
            DialogManager.Instance?.ShowDialog(blockedLines);
            return;
        }

        SceneManager.LoadScene(sceneToLoad);
    }
}
