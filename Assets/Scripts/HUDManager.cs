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
    [SerializeField] private TextMeshProUGUI bossText;

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

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        gold = 0; // reset gold for start of run
        PlayerPrefs.SetInt("Gold", 0);
    }

    private void Start()
    {
        RefreshHUD();
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
        PlayerPrefs.SetInt("Gold", gold); // persist
        Instance?.RefreshHUD();
    }

    public static bool SpendGoldFromTotal(int amount)
    {
        if (gold < amount) return false;

        gold -= amount;
        PlayerPrefs.SetInt("Gold", gold); // persist after spend
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
    }
}