using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialog/Character Dialog Set")]
public class CharacterDialogSetSO : ScriptableObject
{
    public CharacterId character;

    [Serializable]
    public class ContextBlock
    {
        public DialogContext context;
        [TextArea] public List<string> lines;
    }

    public List<ContextBlock> blocks = new List<ContextBlock>();

    public List<string> GetLines(DialogContext context)
    {
        foreach (var block in blocks)
        {
            if (block != null && block.context == context && block.lines != null && block.lines.Count > 0)
            {
                return block.lines;
            }
        }

        return new List<string>
        {
            $"Missing dialog for {character} / {context}."
        };
    }
}

