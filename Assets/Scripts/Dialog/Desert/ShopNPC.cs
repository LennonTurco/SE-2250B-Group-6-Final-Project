using System.Collections.Generic;
using UnityEngine;

public class ShopNPC : MonoBehaviour
{

    [SerializeField] private List<string> shopLines = new List<string>
    {
        "Would you like to see my wares? Press [X]!"
    };

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        DialogManager.Instance.ShowDialog(shopLines);
    }
}