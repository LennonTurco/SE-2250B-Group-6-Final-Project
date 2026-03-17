using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // singleton instance
    public static GameManager Instance { get; private set; }

    // uml fields
    private Level currentLevel;
    private Player player;
    private ProgressionSystem progressionSystem;

    // inspector refs
    [Header("Level Prefabs (index 0-4 = Desert > Jungle > Ice > Volcano > City)")]
    [SerializeField] private LevelManager[] levelPrefabs;
    [SerializeField] private Player playerPrefab;
    [SerializeField] private DialogManager dialogManager;

    // internal state
    private LevelManager currentLevelManager;
    private int currentLevelIndex = -1;
    private bool isTransitioning = false;


    // awake - enforce singleton
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // start - init systems and load first level
    private void Start()
    {
        progressionSystem = new ProgressionSystem();

        player = Instantiate(playerPrefab);
        DontDestroyOnLoad(player.gameObject);

        LoadLevel(0);
    }

    // update - check win/lose each frame
    private void Update()
    {
        if (isTransitioning) return;

        if (player.IsDead())
            HandleLose();
        else if (currentLevelManager != null && currentLevelManager.IsComplete())
            HandleLevelComplete();
    }


    // load level by index
    public void LoadLevel(int index)
    {
        if (index < 0 || index >= levelPrefabs.Length)
        {
            Debug.LogError("[GameManager] LoadLevel: index " + index + " out of range.");
            return;
        }
        StartCoroutine(LoadLevelRoutine(index));
    }

    // tear down current level, collect recruited character
    public void EndLevel()
    {
        if (currentLevelManager == null) return;

        // get recruited character from boss and hand off to other systems
        Boss boss = currentLevelManager.GetBoss();
        if (boss != null)
        {
            Character recruited = boss.GetRecruitedCharacter();
            if (recruited != null)
            {
                progressionSystem.UnlockCharacter(recruited);  // progressionsystem's job
                player.OnCharacterUnlocked(recruited);          // player's job

                // trigger recruitment cutscene
                dialogManager.ShowDialog(new List<string>
                {
                    recruited.characterName + " has joined your party!",
                    "Press 1-5 to swap characters."
                });
            }
        }

        // tear down level
        currentLevelManager.UnloadScene();
        Destroy(currentLevelManager.gameObject);
        currentLevelManager = null;
        currentLevel = null;
    }


    // coroutine - handles the actual level swap
    private IEnumerator LoadLevelRoutine(int index)
    {
        isTransitioning = true;

        // tear down previous level if one exists
        if (currentLevelManager != null)
            EndLevel();

        // instantiate new level and wire dependencies
        currentLevelIndex = index;
        currentLevelManager = Instantiate(levelPrefabs[index]);
        currentLevel = currentLevelManager;
        currentLevelManager.Initialise(player, progressionSystem, dialogManager);
        currentLevelManager.LoadScene();

        yield return new WaitForEndOfFrame();
        isTransitioning = false;
    }

    // check if there is a next level, else trigger win
    private void HandleLevelComplete()
    {
        int next = currentLevelIndex + 1;
        if (next >= levelPrefabs.Length)
            HandleWin();
        else
            LoadLevel(next);
    }

    // game win - show end cutscene
    private void HandleWin()
    {
        dialogManager.ShowDialog(new List<string>
        {
            "Solomon Asantey has been defeated.",
            "The Exile has returned!"
        });
    }

    // game lose - show dialog then respawn
    private void HandleLose()
    {
        isTransitioning = true;
        dialogManager.ShowDialog(new List<string>
        {
            "You Died!",
            "The island reclaims you."
        });
        StartCoroutine(RespawnRoutine());
    }

    // wait then reload current level
    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(2f);
        player.Respawn();           // player handles its own respawn logic
        LoadLevel(currentLevelIndex);
    }


    // accessors for other systems
    public Player GetPlayer() => player;
    public ProgressionSystem GetProgressionSystem() => progressionSystem;
    public LevelManager GetCurrentLevel() => currentLevelManager;
    public int GetCurrentLevelIndex() => currentLevelIndex;
}                        