namespace DefaultNamespace;

public class LevelManager
{
    private List<Tile> tiles;
    private Shop shop;

    public LevelManager()
    {
        tiles = new List<Tile>();
        shop = new Shop();
    }
    public void LoadScene(int levelIndex, string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        shop.LoadStock(levelIndex);
        Debug.Log("Level " + levelIndex + " loaded.");
    }

    public Boolean isComplete()
    {
        foreach (Tile tile in tiles)
        {
            if (tile.IsEntity && !tile.IsInteractable)
                return false;
        }
        return true;
    }
}