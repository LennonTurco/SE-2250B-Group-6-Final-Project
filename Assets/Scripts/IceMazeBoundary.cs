using UnityEngine;
using UnityEngine.Tilemaps;

public class IceMazeBoundary : MonoBehaviour
{
    [Header("Tilemaps")]
    [SerializeField] private Tilemap mazeBoundaries; // the MazeBoundaries tilemap

    [Header("Maze Settings")]
    [SerializeField] private Transform mazeStartPoint; // empty GameObject at maze entrance
    [SerializeField] private float mazeSpeedMultiplier = 2.5f; // extra speed in maze

    private Player player;
    private IcePlayerMovement movement;
    private bool isInMaze = false;
    private float originalMoveSpeed;

    private void Start()
    {
        player = GetComponent<Player>();
        movement = GetComponent<IcePlayerMovement>();
    }

    // if you hit into a boundary, your position is reset to right in front of the maze
    private void Update()
    {
        if (mazeBoundaries == null) return;

        bool onBoundary = IsOnBoundary(transform.position)
                       || IsOnBoundary(transform.position + new Vector3( 0.1f, 0, 0))
                       || IsOnBoundary(transform.position + new Vector3(-0.1f, 0, 0))
                       || IsOnBoundary(transform.position + new Vector3(0,  0.1f, 0))
                       || IsOnBoundary(transform.position + new Vector3(0, -0.1f, 0));

        if (onBoundary && !isInMaze)
        {
            // Player entered the maze - boost speed
            isInMaze = true;
            originalMoveSpeed = player.moveSpeed;
            player.moveSpeed = originalMoveSpeed * mazeSpeedMultiplier;
            Debug.Log("[Maze] Entered maze, speed boosted to " + player.moveSpeed);
        }
        else if (!onBoundary && isInMaze)
        {
            // Player left the maze boundary - respawn at start
            isInMaze = false;
            player.moveSpeed = originalMoveSpeed;

            if (mazeStartPoint != null)
            {
                transform.position = mazeStartPoint.position;
                if (player.rb != null) player.rb.linearVelocity = Vector2.zero;
                Debug.Log("[Maze] Left boundary - respawned at maze start!");
            }
        }
    }

    // checks if you have hit the boundary (outside of the mazeBoundaries)
    private bool IsOnBoundary(Vector3 worldPos)
    {
        Vector3Int cell = mazeBoundaries.WorldToCell(worldPos);
        return mazeBoundaries.HasTile(cell);
    }
}
