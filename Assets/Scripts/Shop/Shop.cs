using System.Collections.Generic;
using UnityEngine;

public class Shop
{
    [System.Serializable]
    public class ShopItem
    {
        public string name;
        public string stat;
        public float amount;
        public int cost;
        public string description;
    }

    private readonly List<ShopItem> stock = new List<ShopItem>();
    private ProgressionSystem progressionSystem;
    public bool IsOpen { get; private set; }

    public void Initialise(ProgressionSystem progression)
    {
        progressionSystem = progression;
    }

    public void LoadStock(int levelIndex)
    {
        stock.Clear();

        switch (levelIndex)
        {
            case 0: // desert
                stock.Add(new ShopItem { name = "D20",          stat = "MaxHP",         amount = 20f, cost = 20, description = "+20 Max HP" });
                stock.Add(new ShopItem { name = "Kunai",         stat = "AttackDamage",  amount = 5f,  cost = 25, description = "+5 Damage" });
                stock.Add(new ShopItem { name = "Runner's Bag",  stat = "MoveSpeed",     amount = 1.2f,cost = 15, description = "1.2X Speed" });
                break;
            case 1: // jungle
                stock.Add(new ShopItem { name = "Silent Footwraps",   stat = "MoveSpeed",    amount = 1f,  cost = 15, description = "Move faster." });
                stock.Add(new ShopItem { name = "Sharpened Shuriken", stat = "AttackDamage", amount = 5f,  cost = 20, description = "Hit harder." });
                break;
            case 2: // ice
                stock.Add(new ShopItem { name = "Ice Armour", stat = "MaxHP",        amount = 20f, cost = 25, description = "Toughen up." });
                break;
            case 3: // volcano
                stock.Add(new ShopItem { name = "Flame Blade", stat = "AttackDamage", amount = 10f, cost = 30, description = "Burn your enemies." });
                break;
            case 4: // city
                stock.Add(new ShopItem { name = "Adrenaline Shot", stat = "AttackCooldown", amount = 0.2f, cost = 35, description = "Attack faster." });
                break;
            default:
                stock.Add(new ShopItem { name = "Generic Upgrade", stat = "MoveSpeed", amount = 1f, cost = 5, description = "A small boost." });
                break;
        }

        Debug.Log("[Shop] Loaded " + stock.Count + " items for level " + levelIndex);
    }

    public void Open()  { IsOpen = true;  Debug.Log("[Shop] Opened."); }
    public void Close() { IsOpen = false; Debug.Log("[Shop] Closed."); }

    public bool Purchase(int index)
    {
        if (!IsOpen)                           { Debug.LogWarning("[Shop] Not open.");           return false; }
        if (index < 0 || index >= stock.Count) { Debug.LogWarning("[Shop] Index out of range."); return false; }

        ShopItem item = stock[index];

        if (!IceHUDManager.SpendGoldFromTotal(item.cost))
        {
            Debug.Log("[Shop] Not enough gold for " + item.name);
            return false;
        }

        // store for apply on return to game scene
        int count = PlayerPrefs.GetInt("PendingCount", 0);
        PlayerPrefs.SetString("PendingStat_" + count, item.stat);
        PlayerPrefs.SetFloat("PendingAmount_" + count, item.amount);
        PlayerPrefs.SetInt("PendingCount", count + 1);

        Debug.Log("[Shop] Purchased: " + item.name);
        return true;
    }

    public List<ShopItem> GetStock() => stock;
    public int GetStockCount() => stock.Count;
}