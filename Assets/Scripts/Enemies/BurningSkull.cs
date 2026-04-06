using UnityEngine;

public class BurningSkull : Enemy
{
    [Header("BurningSkull Settings")]
    [SerializeField] private GameObject firePrefab; // Drag the Fire prefab here in the Inspector
    [SerializeField] private float fireCooldown = 2f;
    [SerializeField] private float fireRange = 6f;
    [SerializeField] private float projectileSpeed = 5f;
    [SerializeField] private float projectileDamage = 10f;
    
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

        Vector2 direction = (target.position - transform.position).normalized;
        Vector2 spawnPosition = (Vector2)transform.position + (direction * 0.6f);
        GameObject fireObj = Instantiate(firePrefab, spawnPosition, Quaternion.identity);

        ShurikenProjectile shurikenProjectile = fireObj.GetComponent<ShurikenProjectile>();
        if (shurikenProjectile != null)
        {
            shurikenProjectile.Initialize(direction, projectileSpeed, projectileDamage, gameObject);
            return;
        }

        Fire fireScript = fireObj.GetComponent<Fire>();
        if (fireScript != null)
        {
            fireScript.dx = direction.x;
            fireScript.dy = direction.y;
            return;
        }

        Rigidbody2D projectileRb = fireObj.GetComponent<Rigidbody2D>();
        if (projectileRb != null)
        {
            projectileRb.linearVelocity = direction * projectileSpeed;
        }
    }
}
