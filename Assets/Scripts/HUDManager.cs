using UnityEngine;
using TMPro;
using System.Collections.Generic;

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
    private int gold = 0;

    [Header("Character Switcher")]
    [SerializeField] private TextMeshProUGUI characterNameText;
    
    [SerializeField] private GameObject Gold1Btn;

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
    }

    private void Start()
    {
        RefreshHUD();
    }

    private void Update()
    {
        RefreshHUD();
    }

    public void RefreshHUD() // this method will be used to update all info displayed in the HUD
    {
        
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

    public void onGoldClicked()
    {
        Gold1Btn.SetActive(false);
        gold += 10;
        goldText.text = "Gold: " + gold;
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
}