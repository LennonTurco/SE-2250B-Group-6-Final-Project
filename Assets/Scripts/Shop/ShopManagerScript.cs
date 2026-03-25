using UnityEngine;
using TMPro;

public class ShopManagerScript : MonoBehaviour
{
    [SerializeField] private TMP_Text coinsTxt;
    [SerializeField] private ShopButtonInfo[] buttons; // drag 3 buttons in inspector

    private Shop shop;

    void Start()
    {
        // get progression system from player in scene
        Player player = FindFirstObjectByType<Player>();
        ProgressionSystem progression = new ProgressionSystem();
        progression.Initialise(player);

        shop = new Shop();
        shop.Initialise(progression);
        shop.LoadStock(PlayerPrefs.GetInt("CurrentLevel", 0)); // level set by levelmanager
        shop.Open();

        RefreshUI();
    }

    // called by each buy button's onClick
    public void Buy(int itemIndex)
    {
        if (shop.Purchase(itemIndex))
        {
            RefreshUI();
        }
    }

    private void RefreshUI()
    {
        coinsTxt.text = "Gold: " + HUDManager.GetGoldTotal();

        var stock = shop.GetStock();
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i < stock.Count)
                buttons[i].SetItem(stock[i].name, stock[i].cost, stock[i].description);
        }
    }
}