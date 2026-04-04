using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class Icicle : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float lifetime = 6f;
    [SerializeField] private float damage = 25f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        CircleCollider2D col = GetComponent<CircleCollider2D>();
        if (col != null) col.isTrigger = true;

        Destroy(gameObject, lifetime);
    }

    public void Launch(Vector2 direction, GameObject owner)
    {
        rb.linearVelocity = direction.normalized * moveSpeed;

        // Ignore collision with the boss that fired it
        Collider2D ownerCol = owner.GetComponent<Collider2D>();
        Collider2D icicleCol = GetComponent<Collider2D>();
        if (ownerCol != null && icicleCol != null)
            Physics2D.IgnoreCollision(icicleCol, ownerCol);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ignore other icicles and enemies
        if (other.GetComponent<Icicle>() != null) return;
        if (other.CompareTag("Enemy")) return;

        // Hit player
        Player player = other.GetComponent<Player>();
        if (player == null) player = other.GetComponentInParent<Player>();
        if (player != null)
        {
            player.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // Destroy on anything solid
        if (!other.isTrigger)
            Destroy(gameObject);
    }
}