using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Strategy object for choosing which dialog lines to show.
/// Implementations are ScriptableObjects so they can be swapped in the inspector.
/// </summary>
public abstract class DialogStrategySO : ScriptableObject
{
    public abstract List<string> GetDialog(CharacterId characterId, DialogContext context, DialogGameState state);
}

