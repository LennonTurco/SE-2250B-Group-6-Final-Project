// // namespace DefaultNamespace;

// public class LevelManager
// {
//     // fields: list of tiles that make up the current level's biome; and shop for this level (each level has a shop)
//     private List<Tile> tiles;
//     private Shop shop;

//     // constructor that initalizes list of tiles and the shop object
//     public LevelManager()
//     {
//         tiles = new List<Tile>();
//         shop = new Shop();
//     }
//     // loads the scene in unity and stocks the shop for the current level 
//     public void LoadScene(int levelIndex, string sceneName)
//     {
//         SceneManager.LoadScene(sceneName);
//         shop.LoadStock(levelIndex);
//         Debug.Log("Level " + levelIndex + " loaded.");
//     }
//     // checks if the tiles for the level have been cleared (means defeated level) to return if level is completed
//     public Boolean isComplete()
//     {
//         // iterates over each tile in this level to check if they have been cleared
//         foreach (Tile tile in tiles)
//         {
//             if (tile.IsEntity && !tile.IsInteractable)
//                 return false;
//         }
//         return true;
//     }
// }