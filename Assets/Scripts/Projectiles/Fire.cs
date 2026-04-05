using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Fire : Entity
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
        // Using Dynamic allows for easier collisions with both kinematic and static objects
        rb.bodyType = RigidbodyType2D.Dynamic; 
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // Better for fast projectiles

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    protected override void Start()
    {
        base.Start();

        if (dx == 0 && dy == 0)
        {
            Debug.LogWarning("Fire projectile has (0,0) direction! It won't move.");
        }

        Vector2 direction = new Vector2(dx, dy).normalized;
        rb.linearVelocity = direction * moveSpeed;
        Destroy(gameObject, lifetime);
    }

    protected override void HandleCollision(GameObject hitObject, bool hitIsTrigger)
    {
        // Don't interact with what fired it
        if (hitObject.CompareTag("Enemy")) return;

        // Use base collision logic (if it hits an Entity, it deals collisionDamage to it)
        base.HandleCollision(hitObject, hitIsTrigger);

        Entity player = hitObject.GetComponentInParent<Player>();
        if (player != null)
        {
            Die();
        }
        else if (!hitIsTrigger && !hitObject.CompareTag("Enemy"))
        {
            Die();
        }
    }
}
