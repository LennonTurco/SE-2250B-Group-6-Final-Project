using System.Collections.Generic;
using UnityEngine;

public class ProgressionSystem
{
    private int progression;
    private List<string> inventory;
    private List<string> unlockedCharacters;

    private Player player; // ref set by gamemanager/levelmanager

    public ProgressionSystem()
    {
        progression = 0;
        inventory = new List<string>();
        unlockedCharacters = new List<string>();
    }

    public void Initialise(Player playerRef)
    {
        player = playerRef;
    }

    // called by shop on purchase - applies upgrade to player immediately
    public void ApplyUpgrade(string stat, float amount)
    {
        if (player == null)
        {
            Debug.LogError("[ProgressionSystem] Player ref is null.");
            return;
        }

        player.ApplyStat(stat, amount);
        inventory.Add(stat + "+" + amount);
        progression++;

        Debug.Log("[ProgressionSystem] Applied: " + stat + " +" + amount);
    }

    // called by gamemanager on boss defeat
    public void UnlockCharacter(string characterName)
    {
        if (unlockedCharacters.Contains(characterName)) return;
        unlockedCharacters.Add(characterName);
        progression++;
        Debug.Log("[ProgressionSystem] Unlocked: " + characterName);
    }

    public int GetProgression() => progression;
    public List<string> GetInventory() => inventory;
    public List<string> GetUnlockedCharacters() => unlockedCharacters;
}