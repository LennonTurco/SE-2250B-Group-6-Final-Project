using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Level-agnostic shop that sells stat upgrades.
/// Pure C# service (no MonoBehaviour) so it can be owned by LevelManager.
/// </summary>
public class Shop
{
    [System.Serializable]
    public class ShopItem
    {
        public string name;
        public string stat;
        public int amount;
        public int cost;
        public string description;
    }

    private readonly List<ShopItem> stock = new List<ShopItem>();
    private int selectedIndex = 0;

    // References provided by GameManager / LevelManager
    private ProgressionSystem progressionSystem;
    private Player player;

    // Simple flag so GameManager can pause updates while the shop is open
    public bool IsOpen { get; private set; }

    public void Initialise(ProgressionSystem progression, Player owningPlayer)
    {
        progressionSystem = progression;
        player = owningPlayer;
    }

    /// <summary>
    /// Populate the shop with level-specific items.
    /// levelIndex: 0-4 = Desert, Jungle, Ice, Volcano, City.
    /// </summary>
    public void LoadStock(int levelIndex)
    {
        stock.Clear();
        selectedIndex = 0;

        // Example stock per level. These can later be moved to ScriptableObjects/data assets.
        switch (levelIndex)
        {
            case 0: // Desert
                stock.Add(new ShopItem
                {
                    name = "Desert Rations",
                    stat = "MaxHP",
                    amount = 1,
                    cost = 10,
                    description = "Slightly increases your max HP."
                });
                break;
            case 1: // Jungle (Neo's level)
                stock.Add(new ShopItem
                {
                    name = "Silent Footwraps",
                    stat = "MoveSpeed",
                    amount = 1,
                    cost = 15,
                    description = "Move faster and stay agile in the trees."
                });
                stock.Add(new ShopItem
                {
                    name = "Sharpened Shuriken",
                    stat = "ShurikenDamage",
                    amount = 1,
                    cost = 20,
                    description = "Your shuriken strike hits harder from the shadows."
                });
                break;
            default:
                stock.Add(new ShopItem
                {
                    name = "Generic Upgrade",
                    stat = "Progression",
                    amount = 1,
                    cost = 5,
                    description = "A small boost to your overall power."
                });
                break;
        }

        Debug.Log("[Shop] Loaded stock for level index " + levelIndex + " (items: " + stock.Count + ")");
    }

    public void Open()
    {
        if (IsOpen) return;
        IsOpen = true;
        Debug.Log("[Shop] Opened.");
    }

    public void Close()
    {
        if (!IsOpen) return;
        IsOpen = false;
        Debug.Log("[Shop] Closed.");
    }

    /// <summary>
    /// Attempt to purchase the currently selected item for the given player.
    /// </summary>
    public void Purchase()
    {
        if (!IsOpen)
        {
            Debug.LogWarning("[Shop] Cannot purchase, shop not open.");
            return;
        }

        if (player == null || progressionSystem == null)
        {
            Debug.LogError("[Shop] Missing references to Player or ProgressionSystem.");
            return;
        }

        if (stock.Count == 0)
        {
            Debug.LogWarning("[Shop] No items in stock.");
            return;
        }

        if (selectedIndex < 0 || selectedIndex >= stock.Count)
        {
            Debug.LogWarning("[Shop] Selected index out of range.");
            return;
        }

        ShopItem item = stock[selectedIndex];

        if (player.Gold < item.cost)
        {
            Debug.Log("[Shop] Not enough gold to purchase " + item.name);
            return;
        }

        player.SpendGold(item.cost);
        progressionSystem.ApplyUpgrade(item.stat, item.amount);

        Debug.Log("[Shop] Purchased " + item.name + " for " + item.cost + " gold.");
    }

    public void SelectNext()
    {
        if (stock.Count == 0) return;
        selectedIndex = (selectedIndex + 1) % stock.Count;
    }

    public void SelectPrevious()
    {
        if (stock.Count == 0) return;
        selectedIndex = (selectedIndex - 1 + stock.Count) % stock.Count;
    }

    public ShopItem GetSelectedItem()
    {
        if (stock.Count == 0 || selectedIndex < 0 || selectedIndex >= stock.Count) return null;
        return stock[selectedIndex];
    }
}

