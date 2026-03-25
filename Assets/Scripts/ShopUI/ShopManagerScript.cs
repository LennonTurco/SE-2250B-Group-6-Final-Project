using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
public class ShopManagerScript : MonoBehaviour
{

    public int[,] shopItems = new int[5,5];
    public TMP_Text CoinsTXT;
    
    void Start()
    {
        RefreshCoinsText();

        //First column is ID's
        shopItems[1,1] = 1;
        shopItems[1,2] = 2;
        shopItems[1,3] = 3;

        //Second column is Price
        shopItems[2,1] = 10;
        shopItems[2,2] = 10;
        shopItems[2,3] = 10;

        //Third column is Quantity
        shopItems[3,1] = 0;
        shopItems[3,2] = 0;
        shopItems[3,3] = 0;     
    }

    public void Buy()
    {
        RefreshCoinsText();
        GameObject ButtonRef = GameObject.FindGameObjectWithTag("Event").GetComponent<EventSystem>().currentSelectedGameObject;
        int itemId = ButtonRef.GetComponent<ShopButtonInfo>().ItemID;
        int price = shopItems[2, itemId];

        if (HUDManager.SpendGoldFromTotal(price))
        {
            shopItems[3, itemId]++;
            RefreshCoinsText();
            ButtonRef.GetComponent<ShopButtonInfo>().QuantityTxt.text = shopItems[3, itemId].ToString();
        }
    }

    private void RefreshCoinsText()
    {
        if (CoinsTXT != null)
        {
            CoinsTXT.text = "Coins: " + HUDManager.GetGoldTotal().ToString();
        }
    }
}
