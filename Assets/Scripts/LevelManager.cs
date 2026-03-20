// using System;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.SceneManagement;
// using UnityEngine.Tilemaps;

// public class LevelManager : MonoBehaviour, Level
// {
//     // fields: list of tiles that make up the current level's biome; and shop for this level (each level has a shop)
//     private readonly List<Tile> tiles = new List<Tile>();
//     private Shop shop;

//     private Player player;
//     private ProgressionSystem progressionSystem;
//     private DialogManager dialogManager;
//     [SerializeField] private string sceneName;
//     [SerializeField] private int levelIndex;
//     private Boss boss;

//     // initializer called from GameManager after instantiation
//     public void Initialise(Player playerRef, ProgressionSystem progressionRef, DialogManager dialogRef)
//     {
//         player = playerRef;
//         progressionSystem = progressionRef;
//         dialogManager = dialogRef;
//         shop = new Shop();
//         shop.Initialise(progressionSystem, player);
//     }

//     // loads the scene in unity and stocks the shop for the current level 
//     public void LoadScene()
//     {
//         SceneManager.LoadScene(sceneName);
//         shop.LoadStock(levelIndex);
//         Debug.Log("Level " + levelIndex + " loaded.");
//     }

//     // checks if the tiles for the level have been cleared (means defeated level) to return if level is completed
//     public bool IsComplete()
//     {
//         // iterates over each tile in this level to check if they have been cleared
//         foreach (Tile tile in tiles)
//         {
//             if (tile.IsEntity && !tile.IsInteractable)
//                 return false;
//         }
//         return true;
//     }

//     // called by GameManager when ending the level
//     public void UnloadScene()
//     {
//         // placeholder for any tear-down logic specific to this level
//         Debug.Log("Level " + levelIndex + " unloaded.");
//     }

//     public Boss GetBoss()
//     {
//         return boss;
//     }
// }

// internal class Boss
// {
// }

// internal interface Level
// {
// }