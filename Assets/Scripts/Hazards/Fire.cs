using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Fire : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float lifetime = 5f;

    [Header("Direction")]
    public float dx;
    public float dy;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f; // Ensure it isn't affected by gravity
    }

    private void Start()
    {
        Vector2 direction = new Vector2(dx, dy).normalized;
        rb.linearVelocity = direction * speed;

        // Self-destruct after lifetime to avoid orphaned objects
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Fire hit something");
        // Check if the collided object has a Player component
        if (collision.TryGetComponent<Player>(out Player player))
        {
            player.TakeDamage(damage);
            Destroy(gameObject);
        }
        // Example: Destroy on walls (any non-trigger collider that isn't the player)
        // If the enemy shoots this, maybe it shouldn't be destroyed by the enemy itself.
        else if (!collision.isTrigger && !collision.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}
