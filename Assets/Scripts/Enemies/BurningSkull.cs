using UnityEngine;

public class BurningSkull : Enemy
{
    [Header("BurningSkull Settings")]
    [SerializeField] private GameObject firePrefab; // Drag the Fire prefab here in the Inspector
    [SerializeField] private float fireCooldown = 2f;
    
    private float fireTimer = 0f;

    protected override void Start()
    {
        base.Start();
        
        // Find the player specifically
        Player player = FindFirstObjectByType<Player>();
        if (player != null)
        {
            target = player.transform;
        }
    }

    protected override void HandleAI()
    {
        if (target == null) return;

        fireTimer += Time.deltaTime;

        if (fireTimer >= fireCooldown)
        {
            fireTimer = 0f;
            ShootFire();
        }
    }

    private void ShootFire()
    {
        if (firePrefab == null)
        {
            Debug.LogWarning("BurningSkull: Fire Prefab is not assigned!");
            return;
        }

        GameObject fireObj = Instantiate(firePrefab, transform.position, Quaternion.identity);
        Fire fireScript = fireObj.GetComponent<Fire>();

        if (fireScript != null)
        {
            // Calculate direction to the target
            Vector2 direction = (target.position - transform.position).normalized;
            
            // Set the dx and dy on the Fire script
            fireScript.dx = direction.x;
            fireScript.dy = direction.y;
        }
    }
}