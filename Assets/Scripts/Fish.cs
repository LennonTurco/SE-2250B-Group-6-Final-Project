using UnityEngine;
using TMPro;

public class Fish : MonoBehaviour
{
    [Header("UI Feedback")]
    [SerializeField] private GameObject noRodPanel;
    [SerializeField] private TextMeshProUGUI noRodText;

    private void Start()
    {
        if (noRodPanel != null) noRodPanel.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (IceInventoryManager.Instance == null) return;

        bool success = IceInventoryManager.Instance.CollectFish();

        if (success)
        {
            // Collected - destroy the fish sprite
            Destroy(gameObject);
        }
        else
        {
            // No fishing rod - show message
            if (noRodPanel != null) noRodPanel.SetActive(true);
            if (noRodText != null)
                noRodText.text = "You need a fishing rod to catch fish!";
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (noRodPanel != null) noRodPanel.SetActive(false);
    }
}
