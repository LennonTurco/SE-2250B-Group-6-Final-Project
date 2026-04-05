using System.Collections.Generic;
using UnityEngine;

public class AntunaDialogueManager : MonoBehaviour
{
    [SerializeField] private AntunaBoss boss;
    
    [SerializeField] private List<string> introLines = new List<string>
    {
        "Who dares enter my domain?",
        "...",
        "You want me to join a wimp like you?",
        "You'll have to prove yourself first.",
        "Prepare for battle!"
    };

    [SerializeField] private List<string> phase2Lines = new List<string>
    {
        "You won't defeat me!"
    };

    [SerializeField] private List<string> defeatLines = new List<string>
    {
        "Impossible...",
        "Well, you have proven yourself. I will join your adventure.",
        "(Press [2] to switch!)"
    };

    private bool hasStartedFight = false;
    private bool hasPlayedIntro = false;
    private bool hasPlayedPhase2 = false;
    private bool hasPlayedDefeat = false;

    private void Update()
    {
        if (!hasStartedFight) return;

        if (boss != null)
        {
            // Start boss fight after intro ends
            if (hasPlayedIntro && !boss.isFighting && !hasPlayedPhase2 && !DialogManager.Instance.IsDisplaying())
            {
                boss.isFighting = true;
            }

            // Fire Phase 2 dialogue if at or below 150 HP
            if (boss.isFighting && !hasPlayedPhase2 && boss.currentHealth <= 300)
            {
                hasPlayedPhase2 = true;
                boss.isFighting = false; // Pause boss
                DialogManager.Instance.ShowDialog(phase2Lines);
            }

            // Resume fight after Phase 2 dialogue ends
            if (hasPlayedPhase2 && !boss.isFighting && !DialogManager.Instance.IsDisplaying())
            {
                boss.isFighting = true;
            }
        }
        else
        {
            if (!hasPlayedDefeat)
            {
                hasPlayedDefeat = true;
                if (DialogManager.Instance != null)
                {
                    DialogManager.Instance.ShowDialog(defeatLines);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasPlayedIntro) return;
        if (!other.CompareTag("Player")) return;

        hasPlayedIntro = true;
        hasStartedFight = true;

        if (boss != null)
        {
            boss.TeleportToCenter();
        }

        if (DialogManager.Instance != null)
        {
            DialogManager.Instance.ShowDialog(introLines);
        }
    }
}
