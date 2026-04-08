using System.Collections.Generic;
using UnityEngine;

public class SphinxNPC : MonoBehaviour
{

    // Sphinx intro dialogue
    [SerializeField] private List<string> clueLines = new List<string>
    {
        "Greetings, traveler. I am the Mega Sphinx.",
        "If you seek my master, you must find my pages three.",
        "The first page is where X marks the spot.",
        "The second page is hidden down an abode's secret path.",
        "The third page is in a guarded oasis."
    };

    // shown if already spoken to
    [SerializeField] private List<string> postPageLines = new List<string>
    {
        "You have located my pages. Now listen closely.",
        "To find my master's lair, search for a skull not like the others.",
        "Hidden in the tall grass nearby lies the way forward.",
        "Prepare well, for he is no pushover."
    };

    // shown if already spoken to
    [SerializeField] private List<string> postDefeatLines = new List<string>
    {
        "Well done, traveler. You defeated my master.",
        "Good luck on your travels."
    };

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (DialogManager.Instance == null || DesertPuzzleManager.Instance == null) return;

        AntunaBoss antuna = FindFirstObjectByType<AntunaBoss>();
        if (antuna == null)
        {
            DialogManager.Instance.ShowDialog(postDefeatLines);
            return;
        }

        if (DesertPuzzleManager.Instance.AllCluesFound())
        {
            DialogManager.Instance.ShowDialog(postPageLines);
            HUDManager.Instance?.SetObjective(HUDManager.Objective.FindAntuna);
        }
        else
        {
            HUDManager.Instance?.SetObjective(HUDManager.Objective.FindPages);
            DialogManager.Instance.ShowDialog(clueLines);
        }

    }
}