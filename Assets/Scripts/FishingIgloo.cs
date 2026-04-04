using UnityEngine;
using TMPro;

public class FishingIgloo : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject dialogPanel;
    [SerializeField] private TextMeshProUGUI dialogText;

    [Header("Settings")]
    [SerializeField] private int requiredGold = 30;

    // if you walk into one of the fishing igloos
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (HUDManager.GetGoldTotal() >= requiredGold)
        {
            // Destroy the igloo
            Destroy(gameObject);
        }
        else
        {
            // Show message
            if (dialogPanel != null) dialogPanel.SetActive(true);
            if (dialogText != null)
                dialogText.text = "You don't have 30 gold yet!";
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Hide message when player walks away
        if (dialogPanel != null) dialogPanel.SetActive(false);
    }
}