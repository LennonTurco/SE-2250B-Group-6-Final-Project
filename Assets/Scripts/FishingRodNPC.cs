using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FishingRodNPC : MonoBehaviour
{
    [Header("Offer Dialog UI")]
    [SerializeField] private GameObject offerPanel;
    [SerializeField] private TextMeshProUGUI offerText;
    [SerializeField] private Button tradeButton;

    [Header("Inventory UI")]
    [SerializeField] private GameObject fishingRodInventoryBox;

    [Header("Settings")]
    [SerializeField] private int cost = 30;

    private bool playerNearby = false;
    private bool alreadyTraded = false;

    private void Start()
    {
        if (offerPanel != null) offerPanel.SetActive(false);
        if (tradeButton != null) tradeButton.onClick.AddListener(OnTrade);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (alreadyTraded) return;

        playerNearby = true;
        ShowOffer();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerNearby = false;
        if (offerPanel != null) offerPanel.SetActive(false);
    }

    private void ShowOffer()
    {
        if (offerPanel != null) offerPanel.SetActive(true);
        if (offerText != null)
            offerText.text = "I surrender! Take it easy...\nI'll trade you a fishing rod for " + cost + " gold.\nUse it at the ice ponds to catch fish!";
        if (tradeButton != null)
            tradeButton.gameObject.SetActive(true);
    }

    private void OnTrade()
    {
        if (HUDManager.GetGoldTotal() >= cost)
        {
            HUDManager.SpendGoldFromTotal(cost);
            alreadyTraded = true;

            // Update dialog
            if (offerText != null)
                offerText.text = "Here you go! Catch 5 fish from\nthe ice ponds to break the big igloo!";

            // Hide trade button
            if (tradeButton != null)
                tradeButton.gameObject.SetActive(false);

            // Show fishing rod in inventory box (bottom right)
            if (fishingRodInventoryBox != null)
                fishingRodInventoryBox.SetActive(true);

            // Close panel after a moment
            Invoke(nameof(ClosePanel), 2.5f);
        }
        else
        {
            if (offerText != null)
                offerText.text = "You don't have enough gold!\nYou need " + cost + " gold.";
        }
    }

    private void ClosePanel()
    {
        if (offerPanel != null) offerPanel.SetActive(false);
    }
}
