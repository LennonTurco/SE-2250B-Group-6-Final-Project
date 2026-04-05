using System.Collections.Generic;
using UnityEngine;

// attach to npc gameobject - needs BoxCollider2D with Is Trigger = true
// The fields should be able to be set in unity for the desired lines
public class DialogTrigger : MonoBehaviour
{
    [SerializeField] private List<string> lines = new List<string>
    {
        "Default NPC line. Set this in the inspector."
    };

    [SerializeField] private bool triggerOnce = true;
    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (triggerOnce && hasTriggered) return;

        hasTriggered = true;
        DialogManager.Instance?.ShowDialog(lines);
    }
}