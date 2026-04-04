using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class Snowball : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float lifetime = 6f;
    [SerializeField] private float damage = 10f;

    private Rigidbody2D rb;

    private void Awake()
    {
        // initiates rigid body and collider
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        CircleCollider2D col = GetComponent<CircleCollider2D>();
        if (col != null) col.isTrigger = true;

        Destroy(gameObject, lifetime);
    }

    // launches a snowball
    public void Launch(Vector2 direction, GameObject owner)
    {
        rb.linearVelocity = direction.normalized * moveSpeed;
    }

    // if the snowball collides with a player, their health is decreased and the snowball is destroyed
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ignore other snowballs
        if (other.GetComponent<Snowball>() != null) return;

        // Ignore enemies/igloos
        if (other.CompareTag("Enemy")) return;

        // Hit player
        Player player = other.GetComponent<Player>();
        if (player == null) player = other.GetComponentInParent<Player>();
        if (player != null)
        {
            player.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}