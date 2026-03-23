using UnityEngine;
using System.Collections.Generic;

// attach to any NPC or zone — set its collider to Is Trigger = true
// covers Task 3 world interaction requirement
public class DialogTrigger : MonoBehaviour
{
    [SerializeField] private List<string> lines = new List<string>
    {
        "A mysterious traveller stands before you.",
        "Defeat the boss to recruit them to your cause."
    };

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;
        triggered = true;

        if (DialogManager.Instance != null)
            DialogManager.Instance.ShowDialog(lines);
        else
            Debug.LogWarning("[DialogTrigger] DialogManager not found in scene.");
    }
}