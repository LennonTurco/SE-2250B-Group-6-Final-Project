using System.Collections.Generic;
using UnityEngine;

public class ProgressionSystem : MonoBehaviour
{
    // singleton instance
    public static ProgressionSystem Instance { get; private set; }

    // fields
    private int progression; // note range is 0-4 (for each level)
    private List<string> inventory;

    // roster of unlocked characters is stored as list. NOTE: May want to rework this and just store as an array later, but should work as a list.
    private List<string> unlockedCharacters;

    // --------------------------------------------------

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        progression = 0;
        inventory = new List<string>();
        unlockedCharacters = new List<string>();
    }

    // apply a stat upgrade by name and amount (called by shop on purchase).
    // the idea here is that each item will have associated stat amounts which will be passed to this method to apply the character upgrades.
    public void ApplyUpgrade(string stat, int amount)
    {
        // track it in inventory
        inventory.Add(stat + "+" + amount);

        // increment progression score
        progression += amount;

        Debug.Log("[ProgressionSystem] Applied upgrade: " + stat + " +" + amount);
    }

    // unlock a recruited character and add to roster (called by gamemanager on boss defeat)
    // ****NOTE MAY NEED TO REWORK THIS IF WE WANT PLAYER TO CALL IT VS CURRENT PLAN WHICH IS FOR BOSS TO CALL IT
    // also may need to alter the parameter to a Player object, as I think Nathan consolidated character and player classes into one, but im going to follow the uml for now
    public void UnlockCharacter(string characterName)
    {
        // changed Character parameter to string for now since Character class doesnt exist yet
        if (unlockedCharacters.Contains(characterName)) return;

        unlockedCharacters.Add(characterName);
        progression++;

        Debug.Log("[ProgressionSystem] Unlocked character: " + characterName);
    }

    // getters
    public int GetProgression() => progression;
    public List<string> GetInventory() => inventory;
    public List<string> GetUnlockedCharacters() => unlockedCharacters;
}