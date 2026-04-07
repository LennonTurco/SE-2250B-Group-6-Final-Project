using UnityEngine;
using TMPro;

// attach to a GO in CityScene - drives the BossText objective for city quest
public class CityQuestTracker : MonoBehaviour
{
    [SerializeField] private TMP_Text objectiveText;

    enum CityPhase { FindInformants, EnterCode, DefeatSolomon }
    private CityPhase phase = CityPhase.FindInformants;
    private int lastClueCount = -1; // track count changes too

    private void Start()
    {
        // restore clues from playerprefs if returning from terminal
        if (PuzzleManager.Instance != null)
        {
            for (int i = 0; i < 3; i++)
            {
                if (PlayerPrefs.GetInt("CityClue_" + i, 0) == 1)
                    PuzzleManager.Instance.RegisterClue(i);
            }
        }

        RefreshPhase();
        UpdateText();
    }

    private void Update()
    {
        CityPhase prev = phase;
        int prevCount = lastClueCount;
        RefreshPhase();

        if (phase != prev || lastClueCount != prevCount)
            UpdateText();
    }

    private void RefreshPhase()
    {
        lastClueCount = CountClues();

        if (PuzzleManager.Instance == null)
        {
            phase = CityPhase.FindInformants;
            return;
        }

        // check static flag first - returning from terminal with code entered
        if (GameState.PuzzleSolved || PuzzleManager.Instance.IsSolved())
            phase = CityPhase.DefeatSolomon;
        else if (PuzzleManager.Instance.AllCluesFound())
            phase = CityPhase.EnterCode;
        else
            phase = CityPhase.FindInformants;
    }

    private void UpdateText()
    {
        if (objectiveText == null) return;

        switch (phase)
        {
            case CityPhase.FindInformants:
                objectiveText.text = "Goal: Find Informants (" + lastClueCount + "/3)";
                break;
            case CityPhase.EnterCode:
                objectiveText.text = "Goal: Enter Code at the Terminal";
                break;
            case CityPhase.DefeatSolomon:
                objectiveText.text = "Goal: Defeat Solomon!";
                break;
        }
    }

    private int CountClues()
    {
        if (PuzzleManager.Instance == null) return 0;
        int count = 0;
        if (PuzzleManager.Instance.HasClue(0)) count++;
        if (PuzzleManager.Instance.HasClue(1)) count++;
        if (PuzzleManager.Instance.HasClue(2)) count++;
        return count;
    }

    // call from CitySceneLoader after solomon spawns
    public void ForceDefeatPhase()
    {
        phase = CityPhase.DefeatSolomon;
        UpdateText();
    }
}