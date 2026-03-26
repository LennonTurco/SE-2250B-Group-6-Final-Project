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

    [Header("Character Switcher")]
    [SerializeField] private TextMeshProUGUI characterNameText;

    // Level and boss names (index 0-4)
    private readonly string[] levelNames = { "Desert", "Jungle", "Ice", "Volcano", "City" };
    private readonly string[] bossNames  =
    {
        "Antuna the Arms Dealer",
        "Keanu the Ninja",
        "Jose the Ice Mage",
        "Amira the Lava Queen",
        "Solomon the Ex-Boss"
    };
    
    private List<string> unlockedCharacters = new List<string> {"Tennon Lurco","Antuna","Keanu","Jose"};
    private int currentCharacterIndex = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        gold = PlayerPrefs.GetInt("Gold", 0); // load persisted gold
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

    public void RefreshHUD() // this method will be used to update all info displayed in the HUD
    {
        if (goldText != null)
        {
            goldText.text = "Gold: " + gold;
        }

        if (characterNameText != null && unlockedCharacters.Count > 0)
        {
            UpdateCharacterDisplay();
        }
        
        if (healthText != null) // updated health hud
        {
        Player player = FindFirstObjectByType<Player>();
        if (player != null)
            healthText.text = "Health: " + player.currentHealth + "/" + player.MaxHealth;
        }
    }
    
    // iterates to the next character in the list
    public void OnNextCharacter()
    {
        if (unlockedCharacters.Count == 0) return;
        currentCharacterIndex = (currentCharacterIndex + 1) % unlockedCharacters.Count;
        UpdateCharacterDisplay();
    }

    // iterates to the prev character in the list
    public void OnPrevCharacter()
    {
        if (unlockedCharacters.Count == 0) return;
        currentCharacterIndex = (currentCharacterIndex - 1 + unlockedCharacters.Count) % unlockedCharacters.Count;
        UpdateCharacterDisplay();
    }

    private void UpdateCharacterDisplay()
    {
        // will be used for fetching the character name (will need to change sprite to in other classes)
        characterNameText.text = "Name: " + unlockedCharacters[currentCharacterIndex];
    }

    // called in different class (probably gameManager) to add a new unlocked character when a level is defeated
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
        if (gold < amount) return false;
        gold -= amount;
        PlayerPrefs.SetInt("Gold", gold); // persist
        Instance?.RefreshHUD();
        return true;
    }

    public int GetGold()
    {
        return gold;
    }

    public static void AddGoldToTotal(int amount)
    {
        gold += amount;
        PlayerPrefs.SetInt("Gold", gold); // persist across scenes
        Instance?.RefreshHUD();
    }

    public static bool SpendGoldFromTotal(int amount)
    {
        if (gold < amount)
        {
            return false;
        }

        gold -= amount;
        if (Instance != null)
        {
            Instance.RefreshHUD();
        }

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
