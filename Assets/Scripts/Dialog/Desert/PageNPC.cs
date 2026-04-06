using System.Collections.Generic;
using UnityEngine;

public class PageA : MonoBehaviour
{
    [SerializeField] private int clueIndex;

    [SerializeField] private List<string> pageLines = new List<string>
    {
        "You found a page!"
    };

    [SerializeField] private List<string> alreadyToldLines = new List<string>
    {
        "You already found this page."
    };

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        // if (DialogManager.Instance == null || PuzzleManager.Instance == null) return;

        if (DesertPuzzleManager.Instance.HasClue(clueIndex))
        {
            return;
        }

        // give the clue and register it
        DialogManager.Instance.ShowDialog(pageLines);
        DesertPuzzleManager.Instance.RegisterClue(clueIndex);

        if (DesertPuzzleManager.Instance.AllCluesFound())
        {
            HUDManager.Instance?.SetObjective(HUDManager.Objective.ReturnToSphinx);
        }
    }
}