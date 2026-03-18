using System.Collections.Generic;

/// <summary>
/// Static helper that holds Level 2 (Jungle) specific dialog content.
/// Other systems can reference these lists when wiring NPCs or cutscenes.
/// </summary>
public static class JungleDialogLibrary
{
    public static readonly List<string> JungleIntro = new List<string>
    {
        "Welcome to the Jungle of Echoes.",
        "Keanu the Ninja watches you from the treetops.",
        "Move silently and strike when he least expects it."
    };

    public static readonly List<string> JungleGymHint = new List<string>
    {
        "See those vines? You can swing between trees to cross the gaps.",
        "Some branches are stronger than others. Watch where you land."
    };

    public static readonly List<string> ShurikenHint = new List<string>
    {
        "Rumour has it a legendary shuriken is hidden deep in the jungle.",
        "From the shadows, its strike is silent but deadly."
    };
}

