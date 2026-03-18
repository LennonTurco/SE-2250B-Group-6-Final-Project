using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Unity-facing router that builds DialogGameState and delegates selection to a strategy asset.
/// NPCs/sprites call RequestDialog through this class.
/// </summary>
public class DialogStrategyRouter : MonoBehaviour
{
    [Header("Strategy")]
    [SerializeField] private DialogStrategySO strategy;

    [Header("Optional state flags (can be set by level scripts)")]
    [SerializeField] private bool jungleBossDefeated;
    [SerializeField] private bool jungleShurikenFound;
    [SerializeField] private bool playerInStealth;

    public List<string> RequestDialog(CharacterId characterId, DialogContext context)
    {
        if (strategy == null)
        {
            return new List<string> { "No dialog strategy assigned." };
        }

        int progression = 0;
        if (GameManager.Instance != null && GameManager.Instance.GetProgressionSystem() != null)
        {
            progression = GameManager.Instance.GetProgressionSystem().GetProgression();
        }

        var state = new DialogGameState(
            progression,
            jungleBossDefeated,
            jungleShurikenFound,
            playerInStealth
        );

        return strategy.GetDialog(characterId, context, state);
    }

    // These setters let other systems update flags without needing direct field access.
    public void SetJungleBossDefeated(bool value) => jungleBossDefeated = value;
    public void SetJungleShurikenFound(bool value) => jungleShurikenFound = value;
    public void SetPlayerInStealth(bool value) => playerInStealth = value;
}

