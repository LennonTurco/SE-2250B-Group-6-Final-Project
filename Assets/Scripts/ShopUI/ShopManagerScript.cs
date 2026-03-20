using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
public class ShopManagerScript : MonoBehaviour
{

    public int[,] shopItems = new int[5,5];
    public float coins;
    public TMP_Text CoinsTXT;
    
    void Start()
    {
        CoinsTXT.text = "Coins:" + coins.ToString();

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
        GameObject ButtonRef = GameObject.FindGameObjectWithTag("Event").GetComponent<EventSystem>().currentSelectedGameObject;

        if (coins >= shopItems[2,ButtonRef.GetComponent<ShopButtonInfo>().ItemID])
        {
            coins -= shopItems[2,ButtonRef.GetComponent<ShopButtonInfo>().ItemID];
            shopItems[3,ButtonRef.GetComponent<ShopButtonInfo>().ItemID]++;
            CoinsTXT.text = "Coins:" + coins.ToString();
            ButtonRef.GetComponent<ShopButtonInfo>().QuantityTxt.text = shopItems[3,ButtonRef.GetComponent<ShopButtonInfo>().ItemID].ToString();


        }
    }
}
