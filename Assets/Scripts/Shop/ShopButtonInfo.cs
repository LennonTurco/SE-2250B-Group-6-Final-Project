using UnityEngine;
using TMPro;

public class ShopButtonInfo : MonoBehaviour
{
    [SerializeField] private TMP_Text nameTxt;
    [SerializeField] private TMP_Text priceTxt;
    [SerializeField] private TMP_Text descTxt;

    public void SetItem(string itemName, int cost, string description)
    {
        nameTxt.text  = itemName;
        priceTxt.text = "Cost: " + cost + "g";
        descTxt.text  = description;
    }
}