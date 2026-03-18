using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialog/Strategies/Simple Character+Context")]
public class SimpleCharacterContextStrategySO : DialogStrategySO
{
    [Serializable]
    public class CharacterEntry
    {
        public CharacterId characterId;
        public CharacterDialogSetSO dialogSet;
    }

    [SerializeField] private List<CharacterEntry> characters = new List<CharacterEntry>();

    public override List<string> GetDialog(CharacterId characterId, DialogContext context, DialogGameState state)
    {
        var set = FindSet(characterId);
        if (set == null)
        {
            return new List<string> { $"Missing dialog set for {characterId}." };
        }

        return set.GetLines(context);
    }

    private CharacterDialogSetSO FindSet(CharacterId characterId)
    {
        foreach (var entry in characters)
        {
            if (entry != null && entry.characterId == characterId)
            {
                return entry.dialogSet;
            }
        }
        return null;
    }
}

