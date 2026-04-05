using System.Collections.Generic;
using UnityEngine;

public class SphinxNPC : MonoBehaviour
{
    [SerializeField] private int clueIndex;

    // Sphinx intro dialogue
    [SerializeField] private List<string> clueLines = new List<string>
    {
        "If you seek Master Antuna, you must find my pages three.",
        "The first page is where X marks the spot.",
        "The second page is with the liar of three.",
        "The third page is with ."
    };

    // shown if already spoken to
    [SerializeField] private List<string> alreadyToldLines = new List<string>
    {
        "You already know what I know. Go."
    };

    private void OnTriggerEnter2D(Collider2D other)
    {
        DialogManager.Instance.ShowDialog(clueLines);
        // if (!other.CompareTag("Player")) return;
        // if (DialogManager.Instance == null || PuzzleManager.Instance == null) return;

        // if (PuzzleManager.Instance.HasClue(clueIndex))
        // {
        //     DialogManager.Instance.ShowDialog(alreadyToldLines);
        //     return;
        // }

        // if (!PuzzleManager.Instance.CanTalkTo(clueIndex))
        // {
        //     DialogManager.Instance.ShowDialog(notReadyLines);
        //     return;
        // }

        // // give the clue and register it
        // DialogManager.Instance.ShowDialog(clueLines);
        // PuzzleManager.Instance.RegisterClue(clueIndex);
    }
}