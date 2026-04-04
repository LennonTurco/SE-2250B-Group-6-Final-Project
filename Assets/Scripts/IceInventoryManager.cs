using UnityEngine;
using TMPro;

public class IceInventoryManager : MonoBehaviour
{
    public static IceInventoryManager Instance { get; private set; }

    [Header("Fishing Rod UI")]
    [SerializeField] private GameObject fishingRodInventoryBox;
    [SerializeField] private TextMeshProUGUI fishingRodCountText;

    [Header("Fish UI")]
    [SerializeField] private GameObject fishInventoryBox;
    [SerializeField] private TextMeshProUGUI fishCountText;

    [Header("Polar Bear Message")]
    [SerializeField] private GameObject polarBearPanel;
    [SerializeField] private TextMeshProUGUI polarBearText;

    [Header("Settings")]
    [SerializeField] private int fishToCollect = 3;

    public int fishingRodCount { get; private set; } = 0;
    public int fishCount { get; private set; } = 0;
    public bool hasIcePickaxe { get; private set; } = false;

    private bool polarBearMessageShown = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        if (fishingRodInventoryBox != null) fishingRodInventoryBox.SetActive(false);
        if (fishInventoryBox != null) fishInventoryBox.SetActive(false);
        if (polarBearPanel != null) polarBearPanel.SetActive(false);
    }

    // Called when player buys a fishing rod from NPC
    public void AddFishingRod()
    {
        fishingRodCount++;
        if (fishingRodInventoryBox != null) fishingRodInventoryBox.SetActive(true);
        RefreshUI();
        Debug.Log("[Inventory] Fishing rods: " + fishingRodCount);
    }

    // Called when player collides with a fish sprite
    // Returns true if successful (player had a fishing rod)
    public bool CollectFish()
    {
        if (fishingRodCount <= 0)
        {
            Debug.Log("[Inventory] No fishing rod to catch fish with!");
            return false;
        }

        // Spend 1 fishing rod, gain 1 fish
        fishingRodCount--;
        fishCount++;

        // Show fish box if not already visible
        if (fishInventoryBox != null) fishInventoryBox.SetActive(true);

        // Hide fishing rod box if no rods left
        if (fishingRodCount <= 0 && fishingRodInventoryBox != null)
            fishingRodInventoryBox.SetActive(false);

        RefreshUI();
        Debug.Log("[Inventory] Fish: " + fishCount + " | Rods left: " + fishingRodCount);

        // Check if player has collected enough fish
        if (fishCount >= fishToCollect && !polarBearMessageShown)
            ShowPolarBearMessage();

        return true;
    }

    public void GiveIcePickaxe()
    {
        hasIcePickaxe = true;
        Debug.Log("[Inventory] Ice Pickaxe obtained!");
    }

    public bool HasFishingRod() => fishingRodCount > 0;
    public bool HasEnoughFish(int required) => fishCount >= required;
    public bool HasIcePickaxe() => hasIcePickaxe;

    private void RefreshUI()
    {
        if (fishingRodCountText != null)
            fishingRodCountText.text = "x" + fishingRodCount;
        if (fishCountText != null)
            fishCountText.text = "x" + fishCount;
    }

    private void ShowPolarBearMessage()
    {
        polarBearMessageShown = true;

        // Hide fish and rod inventory
        if (fishingRodInventoryBox != null) fishingRodInventoryBox.SetActive(false);
        if (fishInventoryBox != null) fishInventoryBox.SetActive(false);

        // Show polar bear message
        if (polarBearPanel != null) polarBearPanel.SetActive(true);
        if (polarBearText != null)
            polarBearText.text = "Mike the Polar Bear: I'm SO hungry...\nI'll trade you my ice pickaxe for those " + fishToCollect + " fish!\nFind my igloo to make the trade!";

        // Auto hide after 5 seconds
        Invoke(nameof(HidePolarBearMessage), 5f);
    }

    private void HidePolarBearMessage()
    {
        if (polarBearPanel != null) polarBearPanel.SetActive(false);
    }
}