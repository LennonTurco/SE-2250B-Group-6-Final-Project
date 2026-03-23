using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ShopManagerScript : MonoBehaviour
{
    public int[,] shopItems = new int[5, 5];
    public float coins;
    public TMP_Text CoinsTXT;

    void Start()
    {
        CoinsTXT.text = "Coins: " + coins.ToString();

        // first column is IDs
        shopItems[1, 1] = 1;
        shopItems[1, 2] = 2;
        shopItems[1, 3] = 3;

        // second column is price
        shopItems[2, 1] = 10;
        shopItems[2, 2] = 10;
        shopItems[2, 3] = 10;

        // third column is quantity owned
        shopItems[3, 1] = 0;
        shopItems[3, 2] = 0;
        shopItems[3, 3] = 0;
    }

    public void Buy()
    {
        GameObject ButtonRef = GameObject.FindGameObjectWithTag("Event")
            .GetComponent<EventSystem>().currentSelectedGameObject;

        int itemID = ButtonRef.GetComponent<ShopButtonInfo>().ItemID;
        int price = shopItems[2, itemID];

        if (coins >= price)
        {
            // deduct cost and update quantity
            coins -= price;
            shopItems[3, itemID]++;

            // update UI
            CoinsTXT.text = "Coins: " + coins.ToString();
            ButtonRef.GetComponent<ShopButtonInfo>().QuantityTxt.text =
                shopItems[3, itemID].ToString();

            // notify progression system — item purchase counts as a gameplay activity
            if (ProgressionSystem.Instance != null)
                ProgressionSystem.Instance.ApplyUpgrade("ShopItem", 1);

            Debug.Log("[ShopManagerScript] Purchased item ID: " + itemID);
        }
        else
        {
            Debug.Log("[ShopManagerScript] Not enough coins.");
        }
    }
}