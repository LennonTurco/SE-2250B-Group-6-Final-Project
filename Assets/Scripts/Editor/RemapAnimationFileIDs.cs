using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class RemapAnimationFileIDs : EditorWindow
{
    [MenuItem("Tools/Remap Animation FileIDs for NinjaGreen")]
    static void RemapNinjaGreenAnimations()
    {
        string character = "NinjaGreen";
        string boyMetaPath = "Assets/Characters/Boy/SpriteSheet.png.meta";
        string charMetaPath = $"Assets/Characters/{character}/SpriteSheet.png.meta";
        string animDir = $"Assets/Characters/{character}/Animations";

        // Get name to fileID mapping for Boy
        Dictionary<string, long> boyNameToFileID = ParseMetaForNameToFileID(boyMetaPath);
        // Get name to fileID mapping for character
        Dictionary<string, long> charNameToFileID = ParseMetaForNameToFileID(charMetaPath);

        // For each .anim file
        string[] animFiles = Directory.GetFiles(animDir, "*.anim");
        foreach (string animFile in animFiles)
        {
            string content = File.ReadAllText(animFile);
            bool changed = false;

            // Find all fileID occurrences
            string[] lines = content.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains("fileID:"))
                {
                    string line = lines[i].Trim();
                    if (line.StartsWith("fileID:"))
                    {
                        string[] parts = line.Split(':');
                        if (long.TryParse(parts[1].Trim(), out long fileID))
                        {
                            // Get name from Boy's mapping
                            var kvp = boyNameToFileID.FirstOrDefault(k => k.Value == fileID);
                            string name = kvp.Key;
                            if (!string.IsNullOrEmpty(name))
                            {
                                // Get new fileID from character's mapping
                                if (charNameToFileID.TryGetValue(name, out long newFileID))
                                {
                                    if (newFileID != fileID)
                                    {
                                        lines[i] = line.Replace(fileID.ToString(), newFileID.ToString());
                                        changed = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (changed)
            {
                File.WriteAllLines(animFile, lines);
                Debug.Log($"Remapped fileIDs in {Path.GetFileName(animFile)}");
            }
        }

        AssetDatabase.Refresh();
        Debug.Log("Remapping complete for NinjaGreen animations!");
    }

    static Dictionary<string, long> ParseMetaForNameToFileID(string metaPath)
    {
        Dictionary<string, long> mapping = new Dictionary<string, long>();
        string[] lines = File.ReadAllLines(metaPath);
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Contains("first:"))
            {
                string firstLine = lines[i + 1].Trim();
                if (firstLine.StartsWith("213:"))
                {
                    long fileID = long.Parse(firstLine.Split(':')[1].Trim());
                    string secondLine = lines[i + 2].Trim();
                    if (secondLine.StartsWith("second:"))
                    {
                        string name = secondLine.Split(':')[1].Trim().Trim('"');
                        mapping[name] = fileID;
                    }
                }
            }
        }
        return mapping;
    }
}