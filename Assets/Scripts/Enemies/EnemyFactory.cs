// Enemy should have a prefab-based factory instead
public class EnemyFactory : MonoBehaviour
{
    [SerializeField] private Enemy[] enemyPrefabs; // assign to some unity prefabs afterwards please

    public Enemy Spawn(int index, Vector3 position)
    {
        if (index < 0 || index >= enemyPrefabs.Length) return null;
        return Instantiate(enemyPrefabs[index], position, Quaternion.identity);
    }
}