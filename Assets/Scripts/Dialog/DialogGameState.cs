/// <summary>
/// Snapshot of game state relevant to dialog selection.
/// Strategies read this to choose which dialog to return.
/// </summary>
public struct DialogGameState
{
    public int progression;

    // Jungle-specific flags (Neo)
    public bool jungleBossDefeated;
    public bool jungleShurikenFound;
    public bool playerInStealth;

    public DialogGameState(
        int progression,
        bool jungleBossDefeated,
        bool jungleShurikenFound,
        bool playerInStealth)
    {
        this.progression = progression;
        this.jungleBossDefeated = jungleBossDefeated;
        this.jungleShurikenFound = jungleShurikenFound;
        this.playerInStealth = playerInStealth;
    }
}

