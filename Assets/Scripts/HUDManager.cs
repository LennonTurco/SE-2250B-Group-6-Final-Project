using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }

    [Header("Health")]
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("Level & Boss")]
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI bossText; // used as objective text

    [Header("Gold")]
    [SerializeField] private TextMeshProUGUI goldText;
    private static int gold = 0;

    [Header("Stats")]
    [SerializeField] private TextMeshProUGUI statsText;

    [Header("Character Switcher")]
    [SerializeField] private TextMeshProUGUI characterNameText;

    private readonly string[] levelNames = { "Desert", "Jungle", "Ice", "Volcano", "City" };
    private readonly string[] bossNames =
    {
        "Antuna the Arms Dealer",
        "Keanu the Ninja",
        "Jose the Ice Mage",
        "Amira the Lava Queen",
        "Solomon the Ex-Boss"
    };

    private List<string> unlockedCharacters = new List<string> { "Tennon Lurco", "Antuna", "Keanu", "Jose" };
    private int currentCharacterIndex = 0;

    // Objective tracking
    public enum Objective
    {
        TalkToSkeleton,
        CollectCoins,
        GoThroughPortal,
        CollectFishingRods,
        FindFish,
        FindPickaxe,
        CompleteIceMaze,
        DefeatBoss
    }

    private Objective currentObjective = Objective.CollectCoins;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        gold = 0;
        PlayerPrefs.SetInt("Gold", 0);
    }

    private void Start()
    {
        RefreshHUD();
        RefreshSceneObjective();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void RefreshHUD()
    {
        UpdateSceneHeaderText();

        if (goldText != null)
            goldText.text = "Gold: " + gold;

        if (characterNameText != null && unlockedCharacters.Count > 0)
            UpdateCharacterDisplay();

        if (healthText != null)
        {
            Player player = FindFirstObjectByType<Player>();
            if (player != null)
                healthText.text = "Health: " + player.currentHealth + "/" + player.MaxHealth;
        }

        if (statsText != null)
        {
            Player player = FindFirstObjectByType<Player>();
            if (player != null)
                statsText.text = "SPD: " + player.moveSpeed +
                                 "\nATK: " + player.attackDamage +
                                 "\nCD: "  + player.attackCooldown;
        }
    }

    // Call this to advance the objective
    public void SetObjective(Objective objective)
    {
        currentObjective = objective;
        UpdateObjectiveText();
    }

    private void UpdateObjectiveText()
    {
        if (bossText == null) return;

        if (SceneManager.GetActiveScene().name == "JungleScene")
        {
            bossText.text = currentObjective == Objective.TalkToSkeleton
                ? "Objective: Talk to Skeleton"
                : "Objective: Solve maze!";
            return;
        }

        if (SceneManager.GetActiveScene().name == "JungleBoss")
        {
            bossText.text = currentObjective == Objective.GoThroughPortal
                ? "Objective: Go through portal"
                : "Objective: Defeat Keanu!";
            return;
        }

        switch (currentObjective)
        {
            case Objective.TalkToSkeleton:
                bossText.text = "Objective: Talk to Skeleton";
                break;
            case Objective.CollectCoins:
                bossText.text = "Objective: Collect Coins";
                break;
            case Objective.GoThroughPortal:
                bossText.text = "Objective: Go through portal";
                break;
            case Objective.CollectFishingRods:
                bossText.text = "Objective: Collect Fishing Rods from the Igloos";
                break;
            case Objective.FindFish:
                bossText.text = "Objective: Find 3 Fishing Rods to Get 3 Fish";
                break;
            case Objective.FindPickaxe:
                bossText.text = "Objective: Find the Pickaxe in the Special Igloo";
                break;
            case Objective.CompleteIceMaze:
                bossText.text = "Objective: Complete the Ice Maze";
                break;
            case Objective.DefeatBoss:
                bossText.text = "Objective: Defeat Jose the Ice Mage by Colliding 5 Times";
                break;
        }
    }

    private void UpdateSceneHeaderText()
    {
        if (levelText == null) return;

        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "JungleScene")
        {
            levelText.text = "Jungle";
            return;
        }

        if (sceneName == "JungleBoss")
        {
            levelText.text = "Jungle Boss";
        }
    }

    public void OnNextCharacter()
    {
        if (unlockedCharacters.Count == 0) return;
        currentCharacterIndex = (currentCharacterIndex + 1) % unlockedCharacters.Count;
        UpdateCharacterDisplay();
    }

    public void OnPrevCharacter()
    {
        if (unlockedCharacters.Count == 0) return;
        currentCharacterIndex = (currentCharacterIndex - 1 + unlockedCharacters.Count) % unlockedCharacters.Count;
        UpdateCharacterDisplay();
    }

    private void UpdateCharacterDisplay()
    {
        characterNameText.text = "Name: " + unlockedCharacters[currentCharacterIndex];
    }

    public void OnCharacterUnlocked(string characterName)
    {
        if (!unlockedCharacters.Contains(characterName))
        {
            unlockedCharacters.Add(characterName);
            Debug.Log("[HUDManager] Character unlocked: " + characterName);
        }
    }

    public void AddGold(int amount)
    {
        AddGoldToTotal(amount);
    }

    public bool SpendGold(int amount)
    {
        return SpendGoldFromTotal(amount);
    }

    public int GetGold()
    {
        return gold;
    }

    public static void AddGoldToTotal(int amount)
    {
        gold += amount;
        PlayerPrefs.SetInt("Gold", gold);
        Instance?.RefreshHUD();

        // Advance objective once player hits 30 gold
        if (gold >= 30 && Instance?.currentObjective == Objective.CollectCoins)
            Instance?.SetObjective(Objective.CollectFishingRods);
    }

    public static bool SpendGoldFromTotal(int amount)
    {
        if (gold < amount) return false;

        gold -= amount;
        PlayerPrefs.SetInt("Gold", gold);
        Instance?.RefreshHUD();
        return true;
    }

    public static int GetGoldTotal()
    {
        return gold;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefreshHUD();
        RefreshSceneObjective();
    }

    public void RefreshSceneObjective()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "JungleScene")
        {
            SetObjective(JungleProgressState.HasSeenSkeletonHint() ? Objective.CollectCoins : Objective.TalkToSkeleton);
            return;
        }

        if (sceneName == "JungleBoss")
        {
            SetObjective(Objective.DefeatBoss);
            return;
        }

        SetObjective(Objective.CollectCoins);
    }
}
