using UnityEngine;

public class DesertBandit : Enemy
{
    [Header("DesertBandit Settings")]
    [SerializeField] private GameObject bulletPrefab; // Drag the Fire prefab here in the Inspector
    [SerializeField] private float fireRange = 12f;
    
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

        float distanceToPlayer = Vector2.Distance(transform.position, target.position);
        if (distanceToPlayer > fireRange)
        {
            fireTimer = 0f;
            return;
        }

        fireTimer += Time.deltaTime;

        if (fireTimer >= attackCooldown)
        {
            fireTimer = 0f;
            ShootFire();
            Invoke(nameof(ShootFire), 0.3f);
            Invoke(nameof(ShootFire), 0.6f);

        }
    }

    private void ShootFire()
    {
        if (bulletPrefab == null)
        {
            Debug.LogWarning("DesertBandit: Fire Prefab is not assigned!");
            return;
        }

        GameObject fireObj = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
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
