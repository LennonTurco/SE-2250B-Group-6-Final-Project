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

    [Header("Special Igloo")]
    [SerializeField] private GameObject specialIgloo;

    [Header("Maze and Boss")]
    [SerializeField] private GameObject mazeBoss;
    [SerializeField] private GameObject joseBoss;

    [Header("Boundaries")]
    [SerializeField] private GameObject mainMapBoundaries;
    [SerializeField] private GameObject fullMapBoundaries;

    [Header("Settings")]
    [SerializeField] private int fishToCollect = 3;

    public int fishingRodCount { get; private set; } = 0;
    public int fishCount { get; private set; } = 0;
    public bool hasIcePickaxe { get; private set; } = false;

    private bool polarBearMessageShown = false;
    
    // ensures only one inventory manager exists to follow singleton pattern
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    // sets all initially inactive features to not be visible so that they are only shown once the inventory follows certain requirements
    private void Start()
    {
        if (fishingRodInventoryBox != null) fishingRodInventoryBox.SetActive(false);
        if (fishInventoryBox != null) fishInventoryBox.SetActive(false);
        if (polarBearPanel != null) polarBearPanel.SetActive(false);
        if (specialIgloo != null) specialIgloo.SetActive(false);
        if (mazeBoss != null) mazeBoss.SetActive(false);
        if (joseBoss != null) joseBoss.SetActive(false);
        if (fullMapBoundaries != null) fullMapBoundaries.SetActive(false);
    }

    // adds fishing rod to inventory if you trade with an npc
    public void AddFishingRod()
    {
        fishingRodCount++;
        if (fishingRodInventoryBox != null) fishingRodInventoryBox.SetActive(true);
        RefreshUI();
        Debug.Log("[Inventory] Fishing rods: " + fishingRodCount);
    }

    // collects fish if you have a fishing rod and collide with a fish
    public bool CollectFish()
    {
        if (fishingRodCount <= 0)
        {
            Debug.Log("[Inventory] No fishing rod to catch fish with!");
            return false;
        }

        fishingRodCount--;
        fishCount++;

        if (fishInventoryBox != null) fishInventoryBox.SetActive(true);

        if (fishingRodCount <= 0 && fishingRodInventoryBox != null)
            fishingRodInventoryBox.SetActive(false);

        RefreshUI();
        Debug.Log("[Inventory] Fish: " + fishCount + " | Rods left: " + fishingRodCount);

        if (fishCount >= fishToCollect && !polarBearMessageShown)
            ShowPolarBearMessage();

        return true;
    }

    // collects pickaxe after you trade for it at the special igloo
    public void GiveIcePickaxe()
    {
        hasIcePickaxe = true;

        if (mazeBoss != null) mazeBoss.SetActive(true);
        if (joseBoss != null) joseBoss.SetActive(true);

        // Swap boundaries
        if (mainMapBoundaries != null) mainMapBoundaries.SetActive(false);
        if (fullMapBoundaries != null) fullMapBoundaries.SetActive(true);

        Debug.Log("[Inventory] Ice Pickaxe obtained! Maze and boss revealed.");
    }

    public bool HasFishingRod() => fishingRodCount > 0;
    public bool HasEnoughFish(int required) => fishCount >= required;
    public bool HasIcePickaxe() => hasIcePickaxe;

    // updates the count of fishing rods and fish in the inventory
    private void RefreshUI()
    {
        if (fishingRodCountText != null)
            fishingRodCountText.text = "x" + fishingRodCount;
        if (fishCountText != null)
            fishCountText.text = "x" + fishCount;
    }

    // once 3 fish are collected, the player is told to go find the special igloo (which is now visible) and trade
    private void ShowPolarBearMessage()
    {
        polarBearMessageShown = true;

        if (fishingRodInventoryBox != null) fishingRodInventoryBox.SetActive(false);
        if (fishInventoryBox != null) fishInventoryBox.SetActive(false);

        if (polarBearPanel != null) polarBearPanel.SetActive(true);
        if (polarBearText != null)
            polarBearText.text = "You have 3 fish!\nFind the special igloo to get the ice pickaxe!";

        if (specialIgloo != null) specialIgloo.SetActive(true);

        Invoke(nameof(HidePolarBearMessage), 5f);
    }
    
    private void HidePolarBearMessage()
    {
        if (polarBearPanel != null) polarBearPanel.SetActive(false);
    }
}