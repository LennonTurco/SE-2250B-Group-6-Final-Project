using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpecialIgloo : MonoBehaviour // only appears once 3 fish are caught and is only used to obtain the pickaxe and open the maze
{
    [Header("Trade UI")]
    [SerializeField] private GameObject tradePanel;
    [SerializeField] private TextMeshProUGUI tradeText;
    [SerializeField] private Button tradeButton;

    [Header("Pickaxe Inventory UI")]
    [SerializeField] private GameObject pickaxeInventoryBox;

    private bool traded = false;

    // makes the special igloo visible if the player has clicked the trade for pickaxe button
    private void Start()
    {
        if (tradePanel != null) tradePanel.SetActive(false);
        if (pickaxeInventoryBox != null) pickaxeInventoryBox.SetActive(false);
        if (tradeButton != null) tradeButton.onClick.AddListener(OnTrade);
    }

    // if the player collides with the special igloo
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (traded) return;

        if (tradePanel != null) tradePanel.SetActive(true);
        if (tradeText != null)
            tradeText.text = "You found the special igloo!\nTrade your 3 fish for an ice pickaxe!";
        if (tradeButton != null)
            tradeButton.gameObject.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (tradePanel != null) tradePanel.SetActive(false);
    }

    // when the trade button is clicked, this adds the pickaxe to the players inventory
    private void OnTrade()
    {
        if (IceInventoryManager.Instance == null) return;

        if (IceInventoryManager.Instance.HasEnoughFish(3))
        {
            traded = true;
            IceInventoryManager.Instance.GiveIcePickaxe();

            gameObject.SetActive(false);

            if (tradeText != null)
                tradeText.text = "You got the ice pickaxe!\nUse it to find the boss!";
            if (tradeButton != null)
                tradeButton.gameObject.SetActive(false);
            if (pickaxeInventoryBox != null)
                pickaxeInventoryBox.SetActive(true);

            Invoke(nameof(ClosePanel), 3f);
        }
        else
        {
            if (tradeText != null)
                tradeText.text = "You don't have enough fish!\nYou need 3 fish.";
        }
    }

    private void ClosePanel()
    {
        if (tradePanel != null) tradePanel.SetActive(false);
    }
}