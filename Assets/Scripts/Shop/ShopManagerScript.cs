using UnityEngine;
using TMPro;

public class ShopManagerScript : MonoBehaviour
{
    [SerializeField] private TMP_Text coinsTxt;
    [SerializeField] private ShopButtonInfo[] buttons;

    private Shop shop;

    void Start()
    {
        Player player = FindFirstObjectByType<Player>();
        ProgressionSystem progression = new ProgressionSystem();
        progression.Initialise(player);

        shop = new Shop();
        shop.Initialise(progression);
        shop.LoadStock(PlayerPrefs.GetInt("CurrentLevel", 0));
        shop.Open();

        RefreshUI();
    }

    public void Buy(int itemIndex)
    {
        if (shop.Purchase(itemIndex))
            RefreshUI();
    }

    private void RefreshUI()
    {
        if (coinsTxt != null)
            coinsTxt.text = "Gold: " + IceHUDManager.GetGoldTotal();

        var stock = shop.GetStock();
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] != null && i < stock.Count)
                buttons[i].SetItem(stock[i].name, stock[i].cost, stock[i].description);
        }
    }
}