using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class TossedCoin : Entity
{
    [Header("Projectile Settings")]
    [SerializeField] private float lifetime = 5f;

    [Header("Direction")]
    public float dx;
    public float dy;

    protected override void Awake()
    {
        base.Awake();
                
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f; 
        rb.bodyType = RigidbodyType2D.Dynamic; 
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }

        moveSpeed = 6f; // A bit faster
        collisionDamage = 10f; // As specified or implied
    }

    protected override void Start()
    {
        base.Start();

        if (dx == 0 && dy == 0)
        {
            dx = 0; dy = -1; // Default downward if entirely zero
        }

        Vector2 direction = new Vector2(dx, dy).normalized;
        rb.linearVelocity = direction * moveSpeed;

        // Clean up after 5 seconds to prevent memory leaks
        Destroy(gameObject, lifetime);
    }

    protected override void HandleCollision(GameObject hitObject, bool hitIsTrigger)
    {
        // Don't deal damage to the Player who threw it
        if (hitObject.CompareTag("Player")) return;

        // Deals the collisionDamage to whatever hitObject is (if it's an Entity)
        base.HandleCollision(hitObject, hitIsTrigger);

        LavaBoss lavaBoss = hitObject.GetComponentInParent<LavaBoss>();
        if (lavaBoss != null)
        {
            lavaBoss.TakeDamage(collisionDamage);
            Die();
            return;
        }

        // Check if we hit an enemy
        Enemy enemy = hitObject.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            Die(); // Destroy the coin after hitting an enemy
        }
        else if (!hitIsTrigger && !hitObject.CompareTag("Player"))
        {
            Die(); // Destroy on walls
        }
    }
}
