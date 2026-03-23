using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class SpriteLoaderByFileID : MonoBehaviour
{
    // Example usage in Editor script
    [MenuItem("Tools/Load Sprite by FileID")]
    static void LoadSpriteExample()
    {
        string texturePath = "Assets/Characters/NinjaGreen/SpriteSheet.png";
        long targetFileID = 5546671543968653177; // Example fileID from .anim

        Sprite sprite = LoadSpriteByFileID(texturePath, targetFileID);
        if (sprite != null)
        {
            Debug.Log("Loaded sprite: " + sprite.name);
        }
        else
        {
            Debug.Log("Sprite not found for fileID: " + targetFileID);
        }
    }

    public static Sprite LoadSpriteByFileID(string texturePath, long fileID)
    {
        // Parse .meta file to get name for fileID
        string metaPath = texturePath + ".meta";
        if (!File.Exists(metaPath)) return null;

        string metaContent = File.ReadAllText(metaPath);
        string search = $"first:\n      213: {fileID}";
        int index = metaContent.IndexOf(search);
        if (index == -1) return null;

        // Find the second: line after
        string after = metaContent.Substring(index);
        int secondIndex = after.IndexOf("second:");
        if (secondIndex == -1) return null;

        string line = after.Substring(secondIndex);
        int start = line.IndexOf('"') + 1;
        int end = line.IndexOf('"', start);
        string spriteName = line.Substring(start, end - start);

        // Load all assets from texture
        Object[] assets = AssetDatabase.LoadAllAssetsAtPath(texturePath);
        return assets.OfType<Sprite>().FirstOrDefault(s => s.name == spriteName);
    }
}