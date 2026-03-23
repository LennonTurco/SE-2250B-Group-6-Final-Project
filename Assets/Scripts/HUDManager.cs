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

    // level and boss names (index 0-4)
    private readonly string[] levelNames = { "Desert", "Jungle", "Ice", "Volcano", "City" };
    private readonly string[] bossNames =
    {
        "Antuna the Arms Dealer",
        "Keanu the Ninja",
        "Jose the Ice Mage",
        "Amira the Lava Queen",
        "Solomon the Ex-Boss"
    };

    private List<string> unlockedCharacters = new List<string> { "Tennon Lurco" };
    private int currentCharacterIndex = 0;

    // current level index — set by GameManager when a level loads
    private int currentLevelIndex = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        // show initial character name on start
        UpdateCharacterDisplay();
        RefreshHUD();
    }

    private void Update()
    {
        // only call RefreshHUD when something changes, not every frame
        // left here as a hook — move logic to event-driven calls later
    }

    // updates all HUD text elements — call this whenever game state changes
    public void RefreshHUD()
    {
        // update level text if ref is assigned
        if (levelText != null)
            levelText.text = "Level " + (currentLevelIndex + 1) + ": " + levelNames[currentLevelIndex];

        // update boss text if ref is assigned
        if (bossText != null)
            bossText.text = "Boss: " + bossNames[currentLevelIndex];

        // update gold text
        if (goldText != null)
            goldText.text = "Gold: " + gold;

        // update progression display if ProgressionSystem exists
        // delegates to ProgressionSystem — not re-implementing its logic here
        if (ProgressionSystem.Instance != null)
            Debug.Log("[HUDManager] Progression: " + ProgressionSystem.Instance.GetProgression());
    }

    // called by GameManager when a new level loads
    public void SetLevel(int index)
    {
        currentLevelIndex = Mathf.Clamp(index, 0, levelNames.Length - 1);
        RefreshHUD();
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
        if (characterNameText == null) return;
        // fetches character name from unlocked list — sprite swap handled elsewhere
        characterNameText.text = "Name: " + unlockedCharacters[currentCharacterIndex];
    }

    public void onGoldClicked()
    {
        if (Gold1Btn != null) Gold1Btn.SetActive(false);
        gold += 10;
        if (goldText != null) goldText.text = "Gold: " + gold;
    }

    // called by GameManager when a boss is defeated and a character is recruited
    public void OnCharacterUnlocked(string characterName)
    {
        if (!unlockedCharacters.Contains(characterName))
        {
            unlockedCharacters.Add(characterName);
            Debug.Log("[HUDManager] Character unlocked: " + characterName);
        }
    }
}