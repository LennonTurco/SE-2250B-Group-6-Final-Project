using System.Collections.Generic;
using UnityEngine;

// the final npc - enter the code here to open the gate
// for now i added to a sprite but when able please wire to a door/barrier via PuzzleManager.OnPuzzleSolved in inspector
public class TerminalNPC : MonoBehaviour
{
    [SerializeField] private List<string> notReadyLines = new List<string>
    {
        "You don't have the full code yet.",
        "Find Solomon's informants scattered through the city."
    };

    [SerializeField] private List<string> solveLines = new List<string>
    {
        "...4... 7... 2...",
        "Access granted. Solomon's compound is open."
    };

    [SerializeField] private List<string> alreadySolvedLines = new List<string>
    {
        "The gate is already open. Go."
    };

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (DialogManager.Instance == null || PuzzleManager.Instance == null) return;

        if (PuzzleManager.Instance.IsSolved())
        {
            DialogManager.Instance.ShowDialog(alreadySolvedLines);
            return;
        }

        if (!PuzzleManager.Instance.AllCluesFound())
        {
            DialogManager.Instance.ShowDialog(notReadyLines);
            return;
        }

        // all clues found - solve and fire gate event
        DialogManager.Instance.ShowDialog(solveLines);
        PuzzleManager.Instance.TrySolve();
    }
}