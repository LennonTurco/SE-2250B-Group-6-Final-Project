using System.Collections.Generic;
using UnityEngine;

// attach to each of the 3 informant npcs
// set clueIndex 0, 1, 2 in inspector for each one
public class InformantNPC : MonoBehaviour
{
    [SerializeField] private int clueIndex;

    // shown before they trust you (wrong order)
    [SerializeField] private List<string> notReadyLines = new List<string>
    {
        "I don't know you. Come back when you know more."
    };

    // the actual clue line - set per npc in inspector
    [SerializeField] private List<string> clueLines = new List<string>
    {
        "The first number is 4. Don't tell anyone I told you."
    };

    // shown if already spoken to
    [SerializeField] private List<string> alreadyToldLines = new List<string>
    {
        "You already know what I know. Go."
    };

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (DialogManager.Instance == null || PuzzleManager.Instance == null) return;

        if (PuzzleManager.Instance.HasClue(clueIndex))
        {
            DialogManager.Instance.ShowDialog(alreadyToldLines);
            return;
        }

        if (!PuzzleManager.Instance.CanTalkTo(clueIndex))
        {
            DialogManager.Instance.ShowDialog(notReadyLines);
            return;
        }

        // give the clue and register it
        DialogManager.Instance.ShowDialog(clueLines);
        PuzzleManager.Instance.RegisterClue(clueIndex);
    }
}