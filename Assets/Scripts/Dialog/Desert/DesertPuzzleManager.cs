using UnityEngine;
using UnityEngine.Events;

// tracks city level informant puzzle state
// lennon owns - wire gate event in inspector
public class DesertPuzzleManager : MonoBehaviour
{
    // public static DesertPuzzleManager Instance { get; private set; }

    // // fired when all clues collected and code entered correctly
    // public UnityEvent OnPuzzleSolved;

    // // track which informants have been spoken to
    // private bool[] cluesFound = new bool[3];
    // private bool puzzleSolved = false;

    // private void Awake()
    // {
    //     if (Instance != null && Instance != this) { Destroy(gameObject); return; }
    //     Instance = this;
    // }

    // // called by each informant npc after their dialog plays
    // public void RegisterClue(int clueIndex)
    // {
    //     if (clueIndex < 0 || clueIndex >= cluesFound.Length) return;
    //     cluesFound[clueIndex] = true;
    //     Debug.Log($"[DesertPuzzleManager] Clue {clueIndex} collected.");
    // }

    // // clue 0 must be found before clue 1, etc.
    // public bool CanTalkTo(int clueIndex)
    // {
    //     if (clueIndex == 0) return true;
    //     return cluesFound[clueIndex - 1];
    // }

    // public bool HasClue(int clueIndex) => cluesFound[clueIndex];

    // public bool AllCluesFound() => cluesFound[0] && cluesFound[1] && cluesFound[2];

    // // called by terminal npc when player submits the code
    // public void TrySolve()
    // {
    //     if (puzzleSolved) return;

    //     if (AllCluesFound())
    //     {
    //         puzzleSolved = true;
    //         Debug.Log("[DesertPuzzleManager] Puzzle solved - opening gate.");
    //         OnPuzzleSolved?.Invoke();
    //     }
    //     else
    //     {
    //         Debug.Log("[DesertPuzzleManager] Not all clues found yet.");
    //     }
    // }

    // public bool IsSolved() => puzzleSolved;
}