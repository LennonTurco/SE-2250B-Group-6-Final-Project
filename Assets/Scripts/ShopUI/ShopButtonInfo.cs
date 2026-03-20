using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ShopButtonInfo : MonoBehaviour
{
    public int ItemID;
    public TMP_Text PriceTxt;
    public TMP_Text QuantityTxt;
    public GameObject ShopManager;


    // Update is called once per frame
    void Update()
    {
        PriceTxt.text = "Price: $" + ShopManager.GetComponent<ShopManagerScript>().shopItems[2,ItemID].ToString();
        QuantityTxt.text = "Quantity: " + ShopManager.GetComponent<ShopManagerScript>().shopItems[3,ItemID].ToString();
    }
}
